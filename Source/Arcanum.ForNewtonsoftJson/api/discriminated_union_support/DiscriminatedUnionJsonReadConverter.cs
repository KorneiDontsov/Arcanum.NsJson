// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

using Arcanum.DataContracts;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	internal sealed class DiscriminatedUnionJsonReadConverter : JsonConverter
	{
		private DiscriminatedUnionInfo _discriminatedUnionInfo { get; }

		public DiscriminatedUnionJsonReadConverter (DiscriminatedUnionInfo discriminatedUnionInfo)
		{
			_discriminatedUnionInfo = discriminatedUnionInfo;
		}

		/// <inheritdoc />
		public override Boolean CanConvert (Type objectType)
		{
			return _discriminatedUnionInfo.dataType.IsAssignableFrom(objectType);
		}

		/// <inheritdoc />
		public override Object? ReadJson (
			JsonReader reader,
			Type objectType,
			Object? existingValue,
			JsonSerializer serializer
		)
		{
			if (reader.TokenType is JsonToken.Null) return null;

			using var discriminatedUnionCaseJsonCache = JsonCacheFactory.shared.GetCache();

			Type discriminatedUnionCaseType;
			using (var discriminatedUnionCaseJsonCacheWriter = discriminatedUnionCaseJsonCache.OpenToWrite())
			{
				discriminatedUnionCaseType = parseDiscriminatedUnionJson(
					_discriminatedUnionInfo,
					reader,
					discriminatedUnionCaseJsonCacheWriter
				);

				// I guess 'objectType' is always assignable from 'discriminatedUnionCaseType'.
			}

			using var discriminatedUnionCaseJsonCacheReader = discriminatedUnionCaseJsonCache.OpenToRead();
			return serializer.Deserialize(discriminatedUnionCaseJsonCacheReader, discriminatedUnionCaseType);
		}

		/// <summary>
		///     Writes to <paramref name="discriminatedUnionCaseJsonWriter" /> case json. Returns case type.
		/// </summary>
		private static Type parseDiscriminatedUnionJson (
			DiscriminatedUnionInfo discriminatedUnionInfo,
			JsonReader discriminatedUnionJsonReader,
			JsonWriter discriminatedUnionCaseJsonWriter
		)
		{
			discriminatedUnionJsonReader.CurrentTokenMustBe(JsonToken.StartObject);
			discriminatedUnionJsonReader.ReadNext();

			Type? discriminatedUnionCaseType = null;

			Boolean? hasValueProperty = null;
			while (discriminatedUnionJsonReader.TokenType != JsonToken.EndObject)
			{
				discriminatedUnionJsonReader.CurrentTokenMustBe(JsonToken.PropertyName);
				var currentPropertyNameString = discriminatedUnionJsonReader.Value.ToString();

				switch (currentPropertyNameString)
				{
					case "$case" when discriminatedUnionCaseType != null:
					{
						throw discriminatedUnionJsonReader.Exception(
							"The property '$case' is contained twice in the object."
						);
					}
					case "$case":
					{
						discriminatedUnionJsonReader.ReadNext();
						discriminatedUnionCaseType = readComplexCaseString();
						break;
					}
					case "$values":
					case "$value":
					{
						switch (hasValueProperty)
						{
							case false:
							{
								throw discriminatedUnionJsonReader.Exception(
									"Cannot deserialize object with '{0}' and its own properties at the same time.",
									currentPropertyNameString
								);
							}
							case true:
							{
								throw discriminatedUnionJsonReader.Exception(
									"The property '{0}' is contained twice in the object.",
									currentPropertyNameString
								);
							}
							case null:
							{
								hasValueProperty = true;
								discriminatedUnionJsonReader.ReadNext();
								discriminatedUnionCaseJsonWriter.WriteToken(
									discriminatedUnionJsonReader,
									writeChildren: true
								);

								break;
							}
						}

						break;
					}
					default:
					{
						switch (hasValueProperty)
						{
							case true:
							{
								throw discriminatedUnionJsonReader.Exception(
									"Cannot deserialize object with '$values' or '$value' and its own properties at the same time."
								);
							}
							case null:
							{
								hasValueProperty = false;
								discriminatedUnionCaseJsonWriter.WriteStartObject();

								goto case false;
							}
							case false:
							{
								discriminatedUnionCaseJsonWriter.WriteToken(discriminatedUnionJsonReader);
								discriminatedUnionJsonReader.ReadNext();
								discriminatedUnionCaseJsonWriter.WriteToken(
									discriminatedUnionJsonReader,
									writeChildren: true
								);

								break;
							}
						}

						break;
					}
				}

				discriminatedUnionJsonReader.ReadNext();
			}

			switch (hasValueProperty)
			{
				case null:
				{
					discriminatedUnionCaseJsonWriter.WriteStartObject();
					discriminatedUnionCaseJsonWriter.WriteEndObject();
					break;
				}
				case false:
				{
					discriminatedUnionCaseJsonWriter.WriteEndObject();
					break;
				}
			}

			_ = discriminatedUnionJsonReader.Read();

			return discriminatedUnionCaseType
			?? throw discriminatedUnionJsonReader.Exception(
				"The property '$case' is not found. It's required to deserialize discriminated union {0} to a specific case.",
				discriminatedUnionInfo.dataType
			);

			// Returns discriminated union case type.
			Type readComplexCaseString ()
			{
				if (discriminatedUnionJsonReader.TokenType != JsonToken.String)
				{
					throw discriminatedUnionJsonReader.Exception(
						"Expected '$case' to be string but accepted {0}",
						discriminatedUnionJsonReader.TokenType
					);
				}

				var complexCaseString = discriminatedUnionJsonReader.Value.ToString();
				if (String.IsNullOrEmpty(complexCaseString))
				{
					throw discriminatedUnionJsonReader.Exception("'$case' is empty string.");
				}

				var caseNameStrings = parseComplexCaseString(complexCaseString);
				return findFinalCase(caseNameStrings);
			}

			List<String> parseComplexCaseString (String complexCaseString)
			{
				var caseNameStrings = new List<String>(4);
				var startIndex = 0;
				for (var endIndex = 0; endIndex < complexCaseString.Length; ++endIndex)
				{
					if (complexCaseString[endIndex] is '.')
					{
						if (endIndex == startIndex)
						{
							throw discriminatedUnionJsonReader.Exception(
								"'$case' is complex case that contains empty part at {0}.",
								endIndex
							);
						}

						caseNameStrings.Add(getCaseNameString(complexCaseString, startIndex, endIndex));
						startIndex = endIndex + 1;
					}
				}

				if (startIndex < complexCaseString.Length)
				{
					caseNameStrings.Add(getCaseNameString(complexCaseString, startIndex, complexCaseString.Length));
				}

				return caseNameStrings;
			}

			String getCaseNameString (String complexCaseString, Int32 startIndex, Int32 endIndex)
			{
				var caseNameString = complexCaseString.Substring(startIndex, endIndex - startIndex);
				if (DataCaseName.TryFindInvalidCharPosition(caseNameString, out var invalidCharPosition))
				{
					throw discriminatedUnionJsonReader.Exception(
						"'$case' contains invalid character at {1} in part '{0}'",
						caseNameString,
						invalidCharPosition
					);
				}

				return caseNameString;
			}

			Type findFinalCase (List<String> caseNameStrings)
			{
				// ReSharper disable once SuggestVarOrType_SimpleTypes
				DiscriminatedUnionInfo? iDiscriminatedUnionInfo = discriminatedUnionInfo.rootUnionInfo;
				Type? iCaseType = null;
				foreach (var caseName in caseNameStrings)
				{
					if (iDiscriminatedUnionInfo is null)
					{
						throw discriminatedUnionJsonReader.Exception(
							"Case {0} is not discriminated union so it cannot have case '{1}'.",
							iCaseType!,
							caseName
						);
					}

					if (
						iDiscriminatedUnionInfo.caseInfosByNames.TryGetValue(caseName, out var caseInfo)
						is false)
					{
						throw discriminatedUnionJsonReader.Exception(
							"Case '{0}' is not found in discriminated union {1}. Known cases: '{2}'.",
							caseName,
							iDiscriminatedUnionInfo.dataType,
							String.Join("', '", iDiscriminatedUnionInfo.caseInfos)
						);
					}

					// ReSharper disable once PossibleNullReferenceException
					iCaseType = caseInfo.dataType;
					iDiscriminatedUnionInfo = caseInfo.asDiscriminatedUnionInfo;
				}

				if (iDiscriminatedUnionInfo != null)
				{
					throw discriminatedUnionJsonReader.Exception(
						"Case of discriminated union {0} is not specified.",
						iDiscriminatedUnionInfo.dataType
					);
				}

				return iCaseType!;
			}
		}

		#region cannot_write
		/// <inheritdoc />
		public override Boolean CanWrite => false;

		/// <inheritdoc />
		public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
}
