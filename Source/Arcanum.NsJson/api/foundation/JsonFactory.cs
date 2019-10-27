// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Annotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;

	public static class JsonFactory {
		#region IJsonSerializer
		static IContractResolver defaultContractResolver { get; } =
			new MiddlewaredJsonContractResolver();

		public static IJsonSerializer defaultSerializer { get; } =
			Serializer(JsonContractResolverConfig.@default, JsonSerializerConfig.@default);

		public static JsonSerializer SerializerCore (
		JsonContractResolverConfig contractResolverConfig, JsonSerializerConfig serializerConfig) {
			var contractResolver = defaultContractResolver;
			var settings =
				new JsonSerializerSettings {
					// important for internal impl
					ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
					// design
					ObjectCreationHandling = ObjectCreationHandling.Auto,
					ConstructorHandling = ConstructorHandling.Default,
					MetadataPropertyHandling = MetadataPropertyHandling.Default,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					NullValueHandling = NullValueHandling.Include,
					DefaultValueHandling = DefaultValueHandling.Include,
					PreserveReferencesHandling = PreserveReferencesHandling.None,
					StringEscapeHandling = StringEscapeHandling.Default,
					FloatFormatHandling = FloatFormatHandling.String,
					FloatParseHandling = FloatParseHandling.Decimal,
					DateFormatHandling = DateFormatHandling.IsoDateFormat,
					DateParseHandling = DateParseHandling.DateTimeOffset,
					DateTimeZoneHandling = DateTimeZoneHandling.Utc,
					// internal configuration
					ContractResolver = contractResolver,
					// culture
					Culture = serializerConfig.culture ?? CultureInfo.InvariantCulture,
					// reference handling
					EqualityComparer = serializerConfig.referenceEqualityComparer,
					ReferenceResolverProvider = serializerConfig.referenceResolverProvider,
					// type handling
					TypeNameHandling = TypeNameHandling.None,
					TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
					SerializationBinder = serializerConfig.serializationBinder,
					// debug
					TraceWriter = serializerConfig.traceWriter
				};
			return JsonSerializer.Create(settings);
		}

		public static JsonSerializer SerializerCore (JsonContractResolverConfig contractResolverConfig) =>
			SerializerCore(contractResolverConfig, JsonSerializerConfig.@default);

		public static JsonSerializer SerializerCore (JsonSerializerConfig serializerConfig) =>
			SerializerCore(JsonContractResolverConfig.@default, serializerConfig);

		public static JsonSerializer SerializerCore () =>
			SerializerCore(JsonContractResolverConfig.@default, JsonSerializerConfig.@default);

		public static IJsonSerializer Serializer (
		JsonContractResolverConfig contractResolverConfig, JsonSerializerConfig serializerConfig) {
			var serializer = SerializerCore(contractResolverConfig, serializerConfig);
			return new JsonSerializerAdapter(serializer);
		}

		public static IJsonSerializer Serializer (JsonContractResolverConfig contractResolverConfig) =>
			Serializer(contractResolverConfig, JsonSerializerConfig.@default);

		public static IJsonSerializer Serializer (JsonSerializerConfig serializerConfig) =>
			Serializer(JsonContractResolverConfig.@default, serializerConfig);
		#endregion

		#region JsonReader
		public static JsonTextReader TextReader (String text) =>
			new JsonTextReader(new StringReader(text));

		public static JTokenReader TokenReader (JToken token) =>
			new JTokenReader(token);
		#endregion

		#region JsonWriter
		public static JsonTextBuilder TextWriter () =>
			new JsonTextBuilder(new StringBuilder());

		public static JsonTextBuilder TextWriter (UInt32 capacity) =>
			new JsonTextBuilder(new StringBuilder((Int32) Math.Min(capacity, Int32.MaxValue)));

		public static JTokenWriter TokenWriter () => new JTokenWriter();
		#endregion

		#region JsonReaderException
		// based on
		// https://github.com/JamesNK/Newtonsoft.Json/blob/febdb8188b226ee7810bbf7e053ada171818fbeb/Src/Newtonsoft.Json/JsonReaderException.cs
		// https://github.com/JamesNK/Newtonsoft.Json/blob/6360cc4c4b4a45f7a82ad6c060cddd9cdb3c7d02/Src/Newtonsoft.Json/JsonPosition.cs
		// 
		// Copyright (c) 2007 James Newton-King

		public static JsonReaderException ReaderException (
		IJsonLineInfo? maybeLineInfo, String path, String message, Exception? innerException = null) {
			static String FormatMessage (IJsonLineInfo? maybeAvailableLineInfo, String path, String message) {
				if (! message.EndsWith(Environment.NewLine, StringComparison.Ordinal)) {
					message = message.Trim();

					if (! message.EndsWith(".", StringComparison.Ordinal)) message += ".";

					message += " ";
				}

				message += String.Format(CultureInfo.InvariantCulture, "Path '{0}'", path);

				if (maybeAvailableLineInfo is { } lineInfo && lineInfo.HasLineInfo())
					message += String.Format(
						CultureInfo.InvariantCulture,
						", line {0}, position {1}",
						maybeAvailableLineInfo.LineNumber,
						maybeAvailableLineInfo.LinePosition
					);

				message += ".";

				return message;
			}

			var maybeAvailableLineInfo = maybeLineInfo?.HasLineInfo() is true ? maybeLineInfo : null;
			var formattedMessage = FormatMessage(maybeAvailableLineInfo, path, message);
			var (lineNumber, linePosition) =
				maybeAvailableLineInfo is { } lineInfo
					? (lineInfo.LineNumber, lineInfo.LinePosition)
					: (0, 0);
			return new JsonReaderException(formattedMessage, path, lineNumber, linePosition, innerException);
		}

		public static JsonReaderException ReaderException (
		JsonReader jsonReader, String message, Exception? innerException) =>
			ReaderException(jsonReader as IJsonLineInfo, jsonReader.Path, message, innerException);

		public static JsonReaderException ReaderException (JsonReader jsonReader, String message) =>
			ReaderException(jsonReader, message, innerException: null);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
		JsonReader jsonReader, IFormatProvider formatProvider, String messageFormat, params Object[] messageArgs) =>
			ReaderException(
				jsonReader,
				String.Format(formatProvider, messageFormat, messageArgs),
				innerException: null);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
		JsonReader jsonReader, String messageFormat, params Object[] messageArgs) =>
			ReaderException(jsonReader, CultureInfo.InvariantCulture, messageFormat, messageArgs);
		#endregion
	}
}
