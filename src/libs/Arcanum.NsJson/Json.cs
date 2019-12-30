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
			standardJsonSerializer.Write(jsonWriter, maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayRead (JsonReader jsonReader, Type dataType) =>
			standardJsonSerializer.MayRead(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T MayRead<T> (JsonReader jsonReader) =>
			standardJsonSerializer.MayRead<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static Object Read (JsonReader jsonReader, Type dataType) =>
			standardJsonSerializer.Read(jsonReader, dataType);

		/// <exception cref = "JsonException" />
		public static T Read<T> (JsonReader jsonReader) =>
			standardJsonSerializer.Read<T>(jsonReader);

		/// <exception cref = "JsonException" />
		public static String ToText (Object? maybeData) =>
			standardJsonSerializer.ToText(maybeData);

		public static String ToText (Object? maybeData, Boolean pretty) =>
			standardJsonSerializer.ToText(maybeData, pretty);

		/// <exception cref = "JsonException" />
		public static Object? MayFromText (String text, Type dataType) =>
			standardJsonSerializer.MayFromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromText<T> (String text) =>
			standardJsonSerializer.MayFromText<T>(text);

		/// <exception cref = "JsonException" />
		public static Object FromText (String text, Type dataType) =>
			standardJsonSerializer.FromText(text, dataType);

		/// <exception cref = "JsonException" />
		public static T FromText<T> (String text) =>
			standardJsonSerializer.FromText<T>(text);

		/// <exception cref = "JsonException" />
		public static JToken ToToken (Object? maybeData) =>
			standardJsonSerializer.ToToken(maybeData);

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken (JToken token, Type dataType) =>
			standardJsonSerializer.MayFromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T MayFromToken<T> (JToken token) =>
			standardJsonSerializer.MayFromToken<T>(token);

		/// <exception cref = "JsonException" />
		public static Object FromToken (JToken token, Type dataType) =>
			standardJsonSerializer.FromToken(token, dataType);

		/// <exception cref = "JsonException" />
		public static T FromToken<T> (JToken token) =>
			standardJsonSerializer.FromToken<T>(token);
	}
}
