// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#pragma warning disable 8601 //disable nullable warnings

namespace Arcanum.NsJson {
	using Arcanum.NsJson.Abstractions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;

	public static class JsonOps {
		/// <exception cref = "JsonException" />
		public static void Write (JsonWriter jsonWriter, Object? maybeData) =>
			JsonFactory.defaultSerializer.Write(jsonWriter, maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayRead (JsonReader jsonReader, Type dataType) =>
			JsonFactory.defaultSerializer.MayRead(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T MayRead<T> (JsonReader jsonReader) =>
			JsonFactory.defaultSerializer.MayRead<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static Object Read (JsonReader jsonReader, Type dataType) =>
			JsonFactory.defaultSerializer.Read(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T Read<T> (JsonReader jsonReader) =>
			JsonFactory.defaultSerializer.Read<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static String ToText (Object? maybeData) =>
			JsonFactory.defaultSerializer.ToText(maybeData);

		public static String ToText (Object? maybeData, Boolean pretty) =>
			JsonFactory.defaultSerializer.ToText(maybeData, pretty);

		/// <exception cref = "JsonException" />
		public static Object? MayFromText (String text, Type dataType) =>
			JsonFactory.defaultSerializer.MayFromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromText<T> (String text) =>
			JsonFactory.defaultSerializer.MayFromText<T>(text);

		/// <exception cref = "JsonException" />
		public static Object FromText (String text, Type dataType) =>
			JsonFactory.defaultSerializer.FromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T FromText<T> (String text) =>
			JsonFactory.defaultSerializer.FromText<T>(text);

		/// <exception cref = "JsonException" />
		public static JToken ToToken (Object? maybeData) =>
			JsonFactory.defaultSerializer.ToToken(maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken (JToken token, Type dataType) =>
			JsonFactory.defaultSerializer.MayFromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromToken<T> (JToken token) =>
			JsonFactory.defaultSerializer.MayFromToken<T>(token);

		/// <exception cref = "JsonException" />
		public static Object FromToken (JToken token, Type dataType) =>
			JsonFactory.defaultSerializer.FromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T FromToken<T> (JToken token) =>
			JsonFactory.defaultSerializer.FromToken<T>(token);
	}
}
