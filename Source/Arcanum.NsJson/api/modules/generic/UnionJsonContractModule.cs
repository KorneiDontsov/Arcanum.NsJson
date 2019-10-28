// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.DataContracts;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;

	class UnionJsonContractModule: IJsonContractGenericModule {
		class Converter: JsonConverter {
			IUnionInfo unionInfo { get; }

			public Converter (IUnionInfo unionInfo) =>
				this.unionInfo = unionInfo;

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) =>
				unionInfo.dataType.IsAssignableFrom(objectType);

			/// <summary>
			///     Writes to <paramref name = "targetWriter" /> case json. Returns case type.
			/// </summary>
			Type SplitUnionJson (JsonReader sourceReader, JsonWriter targetWriter) {
				Type ReadComplexCaseString () {
					if (sourceReader.TokenType != JsonToken.String)
						throw
							sourceReader.Exception(
								"Expected '$case' to be string but accepted {0}",
								sourceReader.TokenType);

					var complexCaseString = sourceReader.Value.ToString();
					if (String.IsNullOrEmpty(complexCaseString))
						throw sourceReader.Exception("'$case' is empty string.");

					var caseNameStrings = ParseComplexCaseString(complexCaseString);
					return FindFinalCase(caseNameStrings);
				}

				List<String> ParseComplexCaseString (String complexCaseString) {
					var caseNameStrings = new List<String>(4);
					var startIndex = 0;
					for (var endIndex = 0; endIndex < complexCaseString.Length; ++ endIndex)
						if (complexCaseString[endIndex] is '.') {
							if (endIndex == startIndex)
								throw
									sourceReader.Exception(
										"'$case' is complex case that contains empty part at {0}.",
										endIndex);

							caseNameStrings.Add(GetCaseNameString(complexCaseString, startIndex, endIndex));
							startIndex = endIndex + 1;
						}

					if (startIndex < complexCaseString.Length)
						caseNameStrings.Add(GetCaseNameString(complexCaseString, startIndex, complexCaseString.Length));

					return caseNameStrings;
				}

				String GetCaseNameString (String complexCaseString, Int32 startIndex, Int32 endIndex) {
					var caseNameString = complexCaseString.Substring(startIndex, endIndex - startIndex);
					return
						UnionCaseUtils.TryFindInvalidCharPosition(caseNameString) is { } invalidCharPosition
							? throw
								sourceReader.Exception(
									"'$case' contains invalid character at {1} in part '{0}'",
									caseNameString,
									invalidCharPosition)
							: caseNameString;
				}

				// Returns discriminated union case type.
				Type FindFinalCase (List<String> caseNameStrings) {
					// ReSharper disable once SuggestVarOrType_SimpleTypes
					IUnionInfo? curUnionInfo = unionInfo.rootUnionInfo;
					Type? curCaseType = null;
					foreach (var caseName in caseNameStrings) {
						if (curUnionInfo is null)
							throw
								sourceReader.Exception(
									"Case {0} is not discriminated union so it cannot have case '{1}'.",
									curCaseType!,
									caseName);

						if (! curUnionInfo.caseInfosByNames.TryGetValue(caseName, out var caseInfo))
							throw
								sourceReader.Exception(
									"Case '{0}' is not found in discriminated union {1}. Known cases: '{2}'.",
									caseName,
									curUnionInfo.dataType,
									String.Join("', '", curUnionInfo.caseInfos));

						// ReSharper disable once PossibleNullReferenceException
						curCaseType = caseInfo.dataType;
						curUnionInfo = caseInfo.asDiscriminatedUnionInfo;
					}

					if (curUnionInfo is { })
						throw
							sourceReader.Exception(
								"Case of discriminated union {0} is not specified.", curUnionInfo.dataType);

					return curCaseType!;
				}

				sourceReader.CurrentTokenMustBe(JsonToken.StartObject);
				sourceReader.ReadNext();

				Type? unionCaseType = null;
				Boolean? hasValueProp = null;

				while (sourceReader.TokenType != JsonToken.EndObject) {
					sourceReader.CurrentTokenMustBe(JsonToken.PropertyName);
					var curPropNameString = sourceReader.Value.ToString();

					switch (curPropNameString) {
						case "$case" when unionCaseType is { }:
							throw
								sourceReader.Exception("The property '$case' is contained twice in the object.");
						case "$case":
							sourceReader.ReadNext();
							unionCaseType = ReadComplexCaseString();
							break;
						case "$values":
						case "$value":
							switch (hasValueProp) {
								case false:
									throw
										sourceReader.Exception(
											"Cannot deserialize object with '{0}' and its own properties at the same time.",
											curPropNameString);
								case true:
									throw
										sourceReader.Exception(
											"The property '{0}' is contained twice in the object.",
											curPropNameString);
								case null:
									hasValueProp = true;
									sourceReader.ReadNext();
									targetWriter.WriteToken(sourceReader, writeChildren: true);
									break;
							}
							break;
						default:
							switch (hasValueProp) {
								case true:
									throw
										sourceReader.Exception(
											"Cannot deserialize object with '$values' or '$value' and its own properties"
											+ " at the same time.");
								case null:
									hasValueProp = false;
									targetWriter.WriteStartObject();
									goto case false;
								case false:
									targetWriter.WriteToken(sourceReader);
									sourceReader.ReadNext();
									targetWriter.WriteToken(sourceReader, writeChildren: true);
									break;
							}
							break;
					}

					sourceReader.ReadNext();
				}
				switch (hasValueProp) {
					case null:
						targetWriter.WriteStartObject();
						targetWriter.WriteEndObject();
						break;
					case false:
						targetWriter.WriteEndObject();
						break;
				}

				return
					unionCaseType
					?? throw
						sourceReader.Exception(
							"The property '$case' is not found. It's required to deserialize discriminated union {0}"
							+ " to a specific case.",
							unionInfo.dataType);
			}

			/// <inheritdoc />
			public override Object? ReadJson (
			JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) {
				if (reader.TokenType is JsonToken.Null) return null;

				using var cache = JsonCacheFactory.shared.GetCache();

				Type unionCaseType; // I guess 'unionCaseType' is always assignable to 'objectType'.
				using (var cacheWriter = cache.OpenToWrite())
					unionCaseType = SplitUnionJson(sourceReader: reader, targetWriter: cacheWriter);

				using var cacheReader = cache.OpenToRead();
				return serializer.Deserialize(cacheReader, unionCaseType);
			}

			#region cannot_write
			/// <inheritdoc />
			public override Boolean CanWrite => false;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) =>
				throw new NotSupportedException();
			#endregion
		}

		/// <inheritdoc />
		public JsonContract? MayCreateContract (JsonContract baseContract) {
			if (baseContract.IsOfAbstractClass() && baseContract.HasNoConverter()
			&& DataTypeInfoStorage.shared.Get(baseContract.UnderlyingType).asUnionInfo is { } unionInfo) {
				if (baseContract.UnderlyingType != unionInfo.dataType)
					throw
						new Exception(
							$"'{nameof(unionInfo)}' {unionInfo} doesn't correspond to 'contract' of type {baseContract.UnderlyingType}.");
				else if (unionInfo.hasErrors)
					throw
						new JsonContractException(
							$"Cannot resolve {unionInfo.dataType} because it has errors.\n{unionInfo.GetErrorString()}");
				else {
					baseContract.Converter = new Converter(unionInfo);
					return baseContract;
				}
			}
			else
				return null;
		}
	}
}
