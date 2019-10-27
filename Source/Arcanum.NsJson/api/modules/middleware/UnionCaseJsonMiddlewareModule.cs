// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.DataContracts;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	class UnionCaseJsonMiddlewareModule: IJsonMiddlewareModule {
		class Converter: JsonConverter {
			IUnionCaseInfo unionCaseInfo { get; }

			String complexCaseString { get; }

			public Converter (IUnionCaseInfo unionCaseInfo) {
				this.unionCaseInfo = unionCaseInfo;
				complexCaseString = ConstructComplexCaseString(unionCaseInfo);
			}

			static String ConstructComplexCaseString (IUnionCaseInfo unionCaseInfo) {
				IEnumerable<String> EnumerateCaseNameStringsFromLeaf (IUnionCaseInfo unionCaseInfo) {
					yield return unionCaseInfo.name.nameString;

					while (unionCaseInfo.maybeDeclaringCaseInfo is { } nextCaseInfo) {
						unionCaseInfo = nextCaseInfo;
						yield return unionCaseInfo.name.nameString;
					}
				}

				var caseNamesFromTop = EnumerateCaseNameStringsFromLeaf(unionCaseInfo).Reverse();
				return String.Join(".", caseNamesFromTop);
			}

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) =>
				unionCaseInfo.dataType.IsAssignableFrom(objectType);

			/// <inheritdoc />
			public override Object? ReadJson (
			JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) =>
				serializer.DeserializeWithoutMiddleware(reader, objectType);

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) {
				static void WriteBaseJson (JsonReader sourceReader, JsonWriter targetWriter) {
					switch (sourceReader.TokenType) {
						case JsonToken.StartObject: {
							sourceReader.ReadNext();

							while (sourceReader.TokenType != JsonToken.EndObject) {
								targetWriter.WriteToken(sourceReader);
								sourceReader.ReadNext();
								targetWriter.WriteToken(sourceReader, writeChildren: true);
								sourceReader.ReadNext();
							}

							break;
						}
						case JsonToken.StartArray: {
							targetWriter.WritePropertyName("$values");
							targetWriter.WriteToken(sourceReader, writeChildren: true);
							break;
						}
						default: {
							targetWriter.WritePropertyName("$value");
							targetWriter.WriteToken(sourceReader);
							break;
						}
					}
				}

				writer.WriteStartObject();
				writer.WritePropertyName("$case");
				writer.WriteValue(complexCaseString);

				using var cache = JsonCacheFactory.shared.GetCache();

				using (var cacheWriter = cache.OpenToWrite())
					serializer.SerializeWithoutMiddleware(cacheWriter, value, unionCaseInfo.dataType);

				using (var cacheReader = cache.OpenToRead())
					WriteBaseJson(sourceReader: cacheReader, targetWriter: writer);

				writer.WriteEndObject();
			}
		}

		/// <inheritdoc />
		public JsonConverter? MayCreateConverter (JsonContract contract) {
			var unionCaseInfo =
				contract.IsOfNonAbstractClass()
					? DataTypeInfoStorage.shared.Get(contract.UnderlyingType).asUnionCaseInfo
					: null;
			if (unionCaseInfo is null)
				return null;
			else if (contract.UnderlyingType != unionCaseInfo.dataType)
				throw
					new Exception(
						$"'{nameof(unionCaseInfo)}' {unionCaseInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}.");
			else
				return new Converter(unionCaseInfo);
		}
	}
}
