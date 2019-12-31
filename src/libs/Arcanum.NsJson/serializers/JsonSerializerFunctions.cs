// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using static Arcanum.NsJson.JsonStreamModule;

	public static class JsonSerializerFunctions {
		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayRead<T> (this IJsonSerializer jsonSerializer, JsonReader jsonReader) =>
			jsonSerializer.MayRead(jsonReader, typeof(T)) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		static Exception DeserializedNullException (Type expectedDataType) =>
			new JsonSerializationException($"'null' was deserialized instead of {expectedDataType}.");

		/// <exception cref = "JsonException" />
		public static Object Read (this IJsonSerializer jsonSerializer, JsonReader jsonReader, Type dataType) =>
			jsonSerializer.MayRead(jsonReader, dataType) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T Read<T> (this IJsonSerializer jsonSerializer, JsonReader jsonReader) =>
			(T) jsonSerializer.Read(jsonReader, typeof(T));

		/// <exception cref = "JsonException" />
		public static String ToText (this IJsonSerializer jsonSerializer, Object? maybeData) {
			using var jsonTextWriter = WriteTextJson();
			jsonSerializer.Write(jsonTextWriter, maybeData);
			return jsonTextWriter.ToString();
		}

		public static String ToText (this IJsonSerializer jsonSerializer, Object? maybeData, Boolean pretty) {
			using var jsonTextWriter = WriteTextJson();
			jsonTextWriter.Formatting = pretty ? Formatting.Indented : Formatting.None;
			jsonSerializer.Write(jsonTextWriter, maybeData);
			return jsonTextWriter.ToString();
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromText (this IJsonSerializer jsonSerializer, String text, Type dataType) {
			using var jsonTextReader = ReadTextJson(text);
			return jsonSerializer.MayRead(jsonTextReader, dataType);
		}

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromText<T> (this IJsonSerializer jsonSerializer, String text) =>
			jsonSerializer.MayFromText(text, typeof(T)) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		public static Object FromText (this IJsonSerializer jsonSerializer, String text, Type dataType) =>
			jsonSerializer.MayFromText(text, dataType) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromText<T> (this IJsonSerializer jsonSerializer, String text) =>
			(T) jsonSerializer.FromText(text, typeof(T));

		/// <exception cref = "JsonException" />
		public static JToken ToToken (this IJsonSerializer jsonSerializer, Object? maybeData) {
			using var jsonTokenWriter = WriteTokenJson();
			jsonSerializer.Write(jsonTokenWriter, maybeData);
			return jsonTokenWriter.Token!;
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken (this IJsonSerializer jsonSerializer, JToken token, Type dataType) {
			using var jsonTokenReader = ReadTokenJson(token);
			return jsonSerializer.MayRead(jsonTokenReader, dataType);
		}

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromToken<T> (this IJsonSerializer jsonSerializer, JToken token) =>
			jsonSerializer.MayFromToken(token, typeof(T)) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		public static Object FromToken (this IJsonSerializer jsonSerializer, JToken token, Type dataType) =>
			jsonSerializer.MayFromToken(token, dataType) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromToken<T> (this IJsonSerializer jsonSerializer, JToken token) =>
			(T) jsonSerializer.FromToken(token, typeof(T));
	}
}
