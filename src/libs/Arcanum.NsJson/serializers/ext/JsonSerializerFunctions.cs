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
		public static T MayRead<T>
			(this IJsonSerializer jsonSerializer,
			 JsonReader jsonReader,
			 Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayRead(jsonReader, typeof(T), configureLocals) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		static Exception DeserializedNullException (Type expectedDataType) =>
			new JsonSerializationException($"'null' was deserialized instead of {expectedDataType}.");

		/// <exception cref = "JsonException" />
		public static Object Read
			(this IJsonSerializer jsonSerializer,
			 JsonReader jsonReader,
			 Type dataType,
			 Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayRead(jsonReader, dataType, configureLocals) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T Read<T>
			(this IJsonSerializer jsonSerializer,
			 JsonReader jsonReader,
			 Action<ILocalsCollection>? configureLocals = null) =>
			(T) jsonSerializer.Read(jsonReader, typeof(T), configureLocals)!;

		/// <exception cref = "JsonException" />
		public static String ToText
			(this IJsonSerializer jsonSerializer,
			 Object? maybeData,
			 Action<ILocalsCollection>? configureLocals = null) {
			using var jsonTextWriter = WriteTextJson();
			jsonSerializer.Write(jsonTextWriter, maybeData, configureLocals);
			return jsonTextWriter.ToString();
		}

		public static String ToText
			(this IJsonSerializer jsonSerializer,
			 Object? maybeData,
			 Boolean pretty,
			 Action<ILocalsCollection>? configureLocals = null) {
			using var jsonTextWriter = WriteTextJson();
			jsonTextWriter.Formatting = pretty ? Formatting.Indented : Formatting.None;
			jsonSerializer.Write(jsonTextWriter, maybeData, configureLocals);
			return jsonTextWriter.ToString();
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromText
			(this IJsonSerializer jsonSerializer,
			 String text,
			 Type dataType,
			 Action<ILocalsCollection>? configureLocals = null) {
			using var jsonTextReader = ReadTextJson(text);
			return jsonSerializer.MayRead(jsonTextReader, dataType, configureLocals);
		}

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromText<T>
			(this IJsonSerializer jsonSerializer, String text, Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayFromText(text, typeof(T), configureLocals) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		public static Object FromText
			(this IJsonSerializer jsonSerializer,
			 String text,
			 Type dataType,
			 Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayFromText(text, dataType, configureLocals) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromText<T>
			(this IJsonSerializer jsonSerializer, String text, Action<ILocalsCollection>? configureLocals = null) =>
			(T) jsonSerializer.FromText(text, typeof(T), configureLocals)!;

		/// <exception cref = "JsonException" />
		public static JToken ToToken
			(this IJsonSerializer jsonSerializer,
			 Object? maybeData,
			 Action<ILocalsCollection>? configureLocals = null) {
			using var jsonTokenWriter = WriteTokenJson();
			jsonSerializer.Write(jsonTokenWriter, maybeData, configureLocals);
			return jsonTokenWriter.Token!;
		}

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken
			(this IJsonSerializer jsonSerializer,
			 JToken token,
			 Type dataType,
			 Action<ILocalsCollection>? configureLocals = null) {
			using var jsonTokenReader = ReadTokenJson(token);
			return jsonSerializer.MayRead(jsonTokenReader, dataType, configureLocals);
		}

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromToken<T>
			(this IJsonSerializer jsonSerializer, JToken token, Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayFromToken(token, typeof(T), configureLocals) is {} data
				? (T) data
				: default;

		/// <exception cref = "JsonException" />
		public static Object FromToken
			(this IJsonSerializer jsonSerializer,
			 JToken token,
			 Type dataType,
			 Action<ILocalsCollection>? configureLocals = null) =>
			jsonSerializer.MayFromToken(token, dataType, configureLocals) is {} data
				? data
				: throw DeserializedNullException(dataType);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromToken<T>
			(this IJsonSerializer jsonSerializer, JToken token, Action<ILocalsCollection>? configureLocals = null) =>
			(T) jsonSerializer.FromToken(token, typeof(T), configureLocals)!;
	}
}
