// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8601 //disable nullable warnings

namespace Arcanum.NsJson.Abstractions {
	using Arcanum.NsJson.Tools;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;

	public static class JsonSerializerUtils {
		/// <exception cref = "JsonException" />
		public static T MayRead<T> (this IJsonSerializer jsonSerializer, JsonReader jsonReader) =>
			(T) jsonSerializer.MayRead(jsonReader, typeof(T));

		/// <exception cref = "JsonException" />
		static Exception DeserializedNullException (Type expectedDataType) =>
			new JsonSerializationException($"'null' was deserialized instead of {expectedDataType}.");

		/// <exception cref = "JsonException" />
		public static Object Read (this IJsonSerializer jsonSerializer, JsonReader jsonReader, Type dataType) =>
			jsonSerializer.MayRead(jsonReader, dataType) is { } data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		public static T Read<T> (this IJsonSerializer jsonSerializer, JsonReader jsonReader) =>
			(T) jsonSerializer.Read(jsonReader, typeof(T));

		/// <exception cref = "JsonException" />
		public static String ToText (this IJsonSerializer jsonSerializer, Object? maybeData) {
			using var jsonTextBuilder = JsonStreamFactory.WriteText();
			jsonSerializer.Write(jsonTextBuilder, maybeData);
			return jsonTextBuilder.ToString();
		}

		public static String ToText (this IJsonSerializer jsonSerializer, Object? maybeData, Boolean pretty) {
			using var jsonTextBuilder = JsonStreamFactory.WriteText();
			jsonTextBuilder.Formatting = pretty ? Formatting.Indented : Formatting.None;
			jsonSerializer.Write(jsonTextBuilder, maybeData);
			return jsonTextBuilder.ToString();
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromText (
		this IJsonSerializer jsonSerializer, String text, Type dataType) {
			using var jsonTextReader = JsonStreamFactory.ReadText(text);
			return jsonSerializer.MayRead(jsonTextReader, dataType);
		}

		/// <exception cref = "JsonException" />
		public static T MayFromText<T> (this IJsonSerializer jsonSerializer, String text) =>
			(T) jsonSerializer.MayFromText(text, typeof(T));

		/// <exception cref = "JsonException" />
		public static Object FromText (this IJsonSerializer jsonSerializer, String text, Type dataType) =>
			jsonSerializer.MayFromText(text, dataType) is { } data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		public static T FromText<T> (this IJsonSerializer jsonSerializer, String text) =>
			(T) jsonSerializer.FromText(text, typeof(T));

		/// <exception cref = "JsonException" />
		public static JToken ToToken (this IJsonSerializer jsonSerializer, Object? maybeData) {
			using var jsonTokenWriter = JsonStreamFactory.WriteToken();
			jsonSerializer.Write(jsonTokenWriter, maybeData);
			return jsonTokenWriter.Token;
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken (this IJsonSerializer jsonSerializer, JToken token, Type dataType) {
			using var jsonTokenReader = JsonStreamFactory.ReadToken(token);
			return jsonSerializer.MayRead(jsonTokenReader, dataType);
		}

		/// <exception cref = "JsonException" />
		public static T MayFromToken<T> (this IJsonSerializer jsonSerializer, JToken token) =>
			(T) jsonSerializer.MayFromToken(token, typeof(T));

		/// <exception cref = "JsonException" />
		public static Object FromToken (this IJsonSerializer jsonSerializer, JToken token, Type dataType) =>
			jsonSerializer.MayFromToken(token, dataType) is { } data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		public static T FromToken<T> (this IJsonSerializer jsonSerializer, JToken token) =>
			(T) jsonSerializer.FromToken(token, typeof(T));
	}
}
