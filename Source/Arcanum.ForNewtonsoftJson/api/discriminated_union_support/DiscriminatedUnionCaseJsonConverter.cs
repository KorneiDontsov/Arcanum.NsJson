// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

using Arcanum.DataContracts;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	internal sealed class DiscriminatedUnionCaseJsonConverter : JsonConverter
	{
		private DiscriminatedUnionCaseInfo _discriminatedUnionCaseInfo { get; }

		private String _complexCaseString { get; }

		public DiscriminatedUnionCaseJsonConverter (DiscriminatedUnionCaseInfo discriminatedUnionCaseInfo)
		{
			_discriminatedUnionCaseInfo = discriminatedUnionCaseInfo;
			_complexCaseString = constructComplexCaseString(discriminatedUnionCaseInfo);
		}

		private static String constructComplexCaseString (DiscriminatedUnionCaseInfo discriminatedUnionCaseInfo)
		{
			var caseInfoStack = new Stack<DiscriminatedUnionCaseInfo>(capacity: 4);

			// ReSharper disable once SuggestVarOrType_SimpleTypes
			for (DiscriminatedUnionCaseInfo? iCaseInfo = discriminatedUnionCaseInfo;
				iCaseInfo != null;
				iCaseInfo = iCaseInfo.maybeDeclaringCaseInfo)
			{
				caseInfoStack.Push(iCaseInfo);
			}

			var sb = new StringBuilder(capacity: 256);

			do
			{
				var caseInfo = caseInfoStack.Pop();
				sb = sb.Append(caseInfo.name).Append('.');
			}
			while (caseInfoStack.Count > 0);

			return sb.ToString(0, sb.Length - 1);
		}

		/// <inheritdoc />
		public override Boolean CanConvert (Type objectType)
		{
			return _discriminatedUnionCaseInfo.dataType.IsAssignableFrom(objectType);
		}

		/// <inheritdoc />
		public override Object? ReadJson (
			JsonReader reader,
			Type objectType,
			Object? existingValue,
			JsonSerializer serializer
		)
		{
			return serializer.DeserializeWithoutMiddleware(reader, objectType);
		}

		/// <inheritdoc />
		public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("$case");
			writer.WriteValue(_complexCaseString);

			using var baseJsonCache = JsonCacheFactory.shared.GetCache();

			using (var baseJsonWriter = baseJsonCache.OpenToWrite())
			{
				serializer.SerializeWithoutMiddleware(baseJsonWriter, value, _discriminatedUnionCaseInfo.dataType);
			}

			using (var baseJsonReader = baseJsonCache.OpenToRead())
			{
				writeBaseJson(baseJsonReader, writer);
			}

			writer.WriteEndObject();


			static void writeBaseJson (JsonReader baseJsonReader, JsonWriter writer)
			{
				switch (baseJsonReader.TokenType)
				{
					case JsonToken.StartObject:
					{
						baseJsonReader.ReadNext();

						while (baseJsonReader.TokenType != JsonToken.EndObject)
						{
							writer.WriteToken(baseJsonReader);
							baseJsonReader.ReadNext();
							writer.WriteToken(baseJsonReader, writeChildren: true);
							baseJsonReader.ReadNext();
						}

						break;
					}
					case JsonToken.StartArray:
					{
						writer.WritePropertyName("$values");
						writer.WriteToken(baseJsonReader, writeChildren: true);
						break;
					}
					default:
					{
						writer.WritePropertyName("$value");
						writer.WriteToken(baseJsonReader);
						break;
					}
				}
			}
		}
	}
}
