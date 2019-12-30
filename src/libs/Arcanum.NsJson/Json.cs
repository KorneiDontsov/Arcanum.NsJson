// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

#nullable disable warnings

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using static Arcanum.NsJson.Module;

	public static class Json {
		/// <exception cref = "JsonException" />
		public static void Write (JsonWriter jsonWriter, Object? maybeData) =>
			defaultJsonSerializer.Write(jsonWriter, maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayRead (JsonReader jsonReader, Type dataType) =>
			defaultJsonSerializer.MayRead(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T MayRead<T> (JsonReader jsonReader) =>
			defaultJsonSerializer.MayRead<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static Object Read (JsonReader jsonReader, Type dataType) =>
			defaultJsonSerializer.Read(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T Read<T> (JsonReader jsonReader) =>
			defaultJsonSerializer.Read<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static String ToText (Object? maybeData) =>
			defaultJsonSerializer.ToText(maybeData);

		public static String ToText (Object? maybeData, Boolean pretty) =>
			defaultJsonSerializer.ToText(maybeData, pretty);

		/// <exception cref = "JsonException" />
		public static Object? MayFromText (String text, Type dataType) =>
			defaultJsonSerializer.MayFromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromText<T> (String text) =>
			defaultJsonSerializer.MayFromText<T>(text);

		/// <exception cref = "JsonException" />
		public static Object FromText (String text, Type dataType) =>
			defaultJsonSerializer.FromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T FromText<T> (String text) =>
			defaultJsonSerializer.FromText<T>(text);

		/// <exception cref = "JsonException" />
		public static JToken ToToken (Object? maybeData) =>
			defaultJsonSerializer.ToToken(maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken (JToken token, Type dataType) =>
			defaultJsonSerializer.MayFromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromToken<T> (JToken token) =>
			defaultJsonSerializer.MayFromToken<T>(token);

		/// <exception cref = "JsonException" />
		public static Object FromToken (JToken token, Type dataType) =>
			defaultJsonSerializer.FromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T FromToken<T> (JToken token) =>
			defaultJsonSerializer.FromToken<T>(token);
	}
}
