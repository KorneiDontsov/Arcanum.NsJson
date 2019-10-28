// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;
	using System.Globalization;

	public static class JsonConfigUtils {
		public static void SetArcaneSettings (this JsonSerializerSettings settings, JsonSerializerConfig config) {
			// important for internal impl
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			// design
			settings.ObjectCreationHandling = ObjectCreationHandling.Auto;
			settings.ConstructorHandling = ConstructorHandling.Default;
			settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;
			settings.MissingMemberHandling = MissingMemberHandling.Ignore;
			settings.NullValueHandling = NullValueHandling.Include;
			settings.DefaultValueHandling = DefaultValueHandling.Include;
			settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			settings.StringEscapeHandling = StringEscapeHandling.Default;
			settings.FloatFormatHandling = FloatFormatHandling.String;
			settings.FloatParseHandling = FloatParseHandling.Decimal;
			settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			settings.DateParseHandling = DateParseHandling.DateTimeOffset;
			settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
			settings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
			// internal configuration
			settings.ContractResolver = JsonContractResolverFactory.@default;
			// culture
			settings.Culture = config.culture ?? CultureInfo.InvariantCulture;
			// constraints
			settings.MaxDepth = config.maxDepth;
			settings.CheckAdditionalContent = config.checkAdditionalContent ?? false;
			// reference handling
			settings.EqualityComparer = config.referenceEqualityComparer;
			settings.ReferenceResolverProvider = config.referenceResolverProvider;
			// type handling
			settings.TypeNameHandling = TypeNameHandling.None;
			settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
			settings.SerializationBinder = config.serializationBinder;
			// debug
			settings.TraceWriter = config.traceWriter;
			//reserved
			settings.Context = default;
			settings.Error = null;
			// ugly and stupid
			settings.Converters = Array.Empty<JsonConverter>();
		}

		public static JsonSerializerSettings CreateNsSerializerSettings (JsonSerializerConfig config) {
			var settings = new JsonSerializerSettings();
			settings.SetArcaneSettings(config);
			return settings;
		}
	}
}
