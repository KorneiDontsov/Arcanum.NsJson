// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using static Arcanum.NsJson.Module;

	public static class Json {
		/// <exception cref = "JsonException" />
		public static void Write
			(JsonWriter jsonWriter, Object? maybeData, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.Write(jsonWriter, maybeData, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object? MayRead
			(JsonReader jsonReader, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayRead(jsonReader, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayRead<T> (JsonReader jsonReader, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayRead<T>(jsonReader, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object Read
			(JsonReader jsonReader, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.Read(jsonReader, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T Read<T> (JsonReader jsonReader, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.Read<T>(jsonReader, configureLocals);

		/// <exception cref = "JsonException" />
		public static String ToText (Object? maybeData, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.ToText(maybeData, configureLocals);

		public static String ToText
			(Object? maybeData, Boolean pretty, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.ToText(maybeData, pretty, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object? MayFromText
			(String text, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayFromText(text, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromText<T> (String text, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayFromText<T>(text, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object FromText (String text, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.FromText(text, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromText<T> (String text, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.FromText<T>(text, configureLocals);

		/// <exception cref = "JsonException" />
		public static JToken ToToken (Object? maybeData, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.ToToken(maybeData, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object? MayFromToken
			(JToken token, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayFromToken(token, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: MaybeNull]
		public static T MayFromToken<T> (JToken token, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.MayFromToken<T>(token, configureLocals);

		/// <exception cref = "JsonException" />
		public static Object FromToken
			(JToken token, Type dataType, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.FromToken(token, dataType, configureLocals);

		/// <exception cref = "JsonException" />
		[return: NotNull]
		public static T FromToken<T> (JToken token, Action<ILocalsCollection>? configureLocals = null) =>
			standardJsonSerializer.FromToken<T>(token, configureLocals);
	}
}
