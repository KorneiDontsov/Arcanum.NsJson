// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;
	using static Newtonsoft.Json.JsonToken;

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
			public void Write (JsonWriter writer, Object value, JsonSerializer serializer, WriteJson previous) {
				using var source = JsonMemory.Rent();

				using (var sourceWriter = source.Write())
					previous(sourceWriter, value, serializer);

				using (var reader = source.Read())
					switch (reader.TokenType) {
						case StartObject:
							reader.ReadNext();

							if (reader.TokenType is EndObject)
								writer.WriteValue(caseRouteStr);
							else {
								writer.WriteStartObject();
								WriteCaseProp(writer);

								while (reader.TokenType != EndObject) {
									reader.CurrentTokenMustBe(PropertyName);
									var propName = (String) reader.Value!;
									if (propName is "$case") {
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
						case StartArray:
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
			var matched = request.dataType.IsClass && ! request.dataType.IsAbstract;
			if (matched && GetDataTypeInfo(request.dataType).asUnionCaseInfo is {} unionCaseInfo)
				try {
					var caseRoute = unionCaseInfo.GetNestedCaseRoute();
					request.Yield(new UnionCaseWriteMiddleware(unionCaseInfo, caseRoute.ToString()));
				}
				catch (FormatException ex) {
					throw new JsonContractException($"Failed to get route of '{unionCaseInfo}'.", ex);
				}
		}
	}
}
