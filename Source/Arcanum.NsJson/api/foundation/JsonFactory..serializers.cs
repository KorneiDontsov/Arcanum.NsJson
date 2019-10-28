﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System.Globalization;

	partial class JsonFactory {
		public static IJsonSerializer defaultSerializer { get; } =
			Serializer(JsonSerializerConfig.@default);

		public static JsonSerializer NsSerializer (JsonSerializerConfig serializerConfig) {
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

		public static JsonSerializer NsSerializer () =>
			NsSerializer(JsonSerializerConfig.@default);

		public static IJsonSerializer Serializer (JsonSerializerConfig serializerConfig) {
			var serializer = NsSerializer(serializerConfig);
			return new JsonSerializerAdapter(serializer);
		}
	}
}