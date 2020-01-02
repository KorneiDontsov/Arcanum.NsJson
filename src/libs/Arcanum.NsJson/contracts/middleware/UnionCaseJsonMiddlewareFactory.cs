// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using Arcanum.Routes;
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;

	public sealed class UnionCaseJsonMiddlewareFactory: IJsonMiddlewareFactory {
		class UnionCaseWriteMiddleware: IJsonWriteMiddleware {
			String caseRouteStr { get; }

			public UnionCaseWriteMiddleware (Route caseRoute) => caseRouteStr = caseRoute.ToString();

			/// <inheritdoc />
			public void WriteJson (JsonWriter writer, Object value, JsonSerializer serializer, WriteJson previous) {
				static void WriteValue (JsonReader input, JsonWriter output) {
					switch (input.TokenType) {
						case JsonToken.StartObject:
							input.ReadNext();
							while (input.TokenType != JsonToken.EndObject) {
								output.WriteToken(input);
								input.ReadNext();
								output.WriteToken(input, writeChildren: true);
								input.ReadNext();
							}
							break;
						case JsonToken.StartArray:
							output.WritePropertyName("$values");
							output.WriteToken(input, writeChildren: true);
							break;
						default:
							output.WritePropertyName("$value");
							output.WriteToken(input);
							break;
					}
				}

				writer.WriteStartObject();
				writer.WritePropertyName("$case");
				writer.WriteValue(caseRouteStr);

				using (var source = JsonMemory.Rent()) {
					using (var sourceWriter = source.Write())
						previous(sourceWriter, value, serializer);

					using (var reader = source.Read())
						WriteValue(input: reader, output: writer);
				}

				writer.WriteEndObject();
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonMiddlewareRequest request) {
			var matched = request.dataType.IsClass && ! request.dataType.IsAbstract;
			if (matched && GetDataTypeInfo(request.dataType).asUnionCaseInfo is {} unionCaseInfo)
				try {
					var caseRoute = unionCaseInfo.GetNestedCaseRoute();
					request.Yield(new UnionCaseWriteMiddleware(caseRoute));
				}
				catch (FormatException ex) {
					throw new JsonContractException($"Failed to get route of '{unionCaseInfo}'.", ex);
				}
		}
	}
}
