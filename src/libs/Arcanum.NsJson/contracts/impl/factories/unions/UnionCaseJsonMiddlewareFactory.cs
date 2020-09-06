// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;

	public sealed class UnionCaseJsonMiddlewareFactory: IJsonMiddlewareFactory {
		class UnionCaseWriteMiddleware: IToJsonMiddleware {
			IUnionCaseInfo unionCaseInfo { get; }
			String caseRouteStr { get; }

			public UnionCaseWriteMiddleware (IUnionCaseInfo unionCaseInfo, String caseRouteStr) {
				this.unionCaseInfo = unionCaseInfo;
				this.caseRouteStr = caseRouteStr;
			}

			void WriteCaseProp (JsonWriter writer) {
				writer.WritePropertyName("$case");
				writer.WriteValue(caseRouteStr);
			}

			/// <inheritdoc />
			public void Write
				(IJsonSerializer serializer,
				 JsonWriter writer,
				 Object value,
				 WriteJson previous,
				 ILocalsCollection locals) {
				using var source = JsonMemory.Rent();

				using(var sourceWriter = source.Write())
					previous(serializer, sourceWriter, value);

				using(var reader = source.Read())
					switch(reader.TokenType) {
						case JsonToken.Null:
							writer.WriteNull();
							break;
						case JsonToken.StartObject:
							reader.ReadNext();

							if(reader.TokenType is JsonToken.EndObject)
								writer.WriteValue(caseRouteStr);
							else {
								writer.WriteStartObject();
								WriteCaseProp(writer);

								while(reader.TokenType != JsonToken.EndObject) {
									reader.CurrentTokenMustBe(JsonToken.PropertyName);
									var propName = (String) reader.Value!;
									if(propName is "$case") {
										var msg =
											$"Failed to serialize '{unionCaseInfo}' because it has property '$case' "
											+ "which is not allowed.";
										throw new JsonSerializationException(msg);
									}
									else {
										writer.WriteToken(reader);
										reader.ReadNext();
										writer.WriteToken(reader, writeChildren: true);
										reader.ReadNext();
									}
								}

								writer.WriteEndObject();
							}

							break;
						case JsonToken.StartArray:
							writer.WriteStartObject();
							WriteCaseProp(writer);
							writer.WritePropertyName("$values");
							writer.WriteToken(reader, writeChildren: true);
							writer.WriteEndObject();
							break;
						default:
							writer.WriteStartObject();
							WriteCaseProp(writer);
							writer.WritePropertyName("$value");
							writer.WriteToken(reader);
							writer.WriteEndObject();
							break;
					}
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonMiddlewareRequest request) {
			if(request.dataType.IsClass
			   && ! request.dataType.IsAbstract
			   && ! request.dataType.IsGenericTypeDefinition
			   && GetDataTypeInfo(request.dataType).asUnionCaseInfo is {} unionCaseInfo)
				try {
					var caseRoute = unionCaseInfo.GetNestedCaseRoute();
					request.Yield(new UnionCaseWriteMiddleware(unionCaseInfo, caseRoute.ToString()));
				}
				catch(FormatException ex) {
					throw new JsonContractException($"Failed to get route of '{unionCaseInfo}'.", ex);
				}
		}
	}
}
