// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Globalization;
	using static Arcanum.NsJson.Contracts.Module;

	public static class Module {
		public static void SetUpNsSerializerSettings
			(this JsonSerializerSetup setup, JsonSerializerSettings settings, JsonSerializerConfig? config = null) {
			static Int32? ConvertDepth (UInt32 depth) =>
				depth switch {
					0 => (Int32?) null,
					_ when depth <= Int32.MaxValue => (Int32) depth,
					_ => Int32.MaxValue
				};

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
			settings.TypeNameHandling = TypeNameHandling.None;
			settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
			// culture
			settings.Culture = config?.culture ?? setup.culture;
			// constraints
			settings.MaxDepth =
				ConvertDepth(
					config?.maxDepth is var configMaxDepth && configMaxDepth is { }
						? configMaxDepth.GetValueOrDefault()
						: setup.maxDepth);
			settings.CheckAdditionalContent = config?.checkAdditionalContent ?? setup.checkAdditionalContent;
			// reference handling
			settings.EqualityComparer = config?.referenceComparer ?? setup.referenceComparer;
			settings.ReferenceResolverProvider = config?.referenceResolverProvider ?? setup.referenceResolverProvider;
			// type handling
			settings.SerializationBinder = config?.serializationBinder ?? setup.serializationBinder;
			// debug
			settings.TraceWriter = config?.traceWriter;
			//reserved
			settings.Context = default;
			settings.Error = null;
			// ugly and stupid
			settings.Converters = Array.Empty<JsonConverter>();
		}

		public static JsonSerializerSettings CreateNsSerializerSettings
			(this JsonSerializerSetup setup, IContractResolver contractResolver, JsonSerializerConfig? config = null) {
			var settings = new JsonSerializerSettings { ContractResolver = contractResolver };
			setup.SetUpNsSerializerSettings(settings, config);
			return settings;
		}

		public static JsonSerializer CreateNsJsonSerializer
			(this JsonSerializerSetup setup, IContractResolver contractResolver, JsonSerializerConfig? config = null) {
			var settings = setup.CreateNsSerializerSettings(contractResolver, config);
			return JsonSerializer.Create(settings);
		}

		public static IJsonSerializer CreateJsonSerializer
			(this JsonSerializerSetup setup, IContractResolver contractResolver, JsonSerializerConfig? config = null) {
			var nsSerializer = setup.CreateNsJsonSerializer(contractResolver, config);
			return new JsonSerializerAdapter(nsSerializer);
		}

		public static JsonSerializerSetup standardJsonSerializerSetup { get; } =
			new JsonSerializerSetup(
				CultureInfo.InvariantCulture,
				maxDepth: 32,
				checkAdditionalContent: false,
				referenceComparer: new DefaultReferenceComparer(),
				referenceResolverProvider: () => new JsonSerializer().ReferenceResolver!,
				serializationBinder: new JsonSerializer().SerializationBinder);

		public static IContractResolver standardContractResolver { get; } =
			CreateMicroContractResolverBuilder()
				.AddStandardContracts()
				.AddPlatformContracts()
				.Build();

		public static IJsonSerializer standardJsonSerializer { get; } =
			new JsonSerializerAdapter(
				serializer: standardJsonSerializerSetup.CreateNsJsonSerializer(standardContractResolver));

		public static JsonSerializer CreateNsJsonSerializer (JsonSerializerConfig? serializerConfig = null) =>
			standardJsonSerializerSetup.CreateNsJsonSerializer(standardContractResolver, serializerConfig);

		public static IJsonSerializer CreateJsonSerializer (JsonSerializerConfig serializerConfig) =>
			standardJsonSerializerSetup.CreateJsonSerializer(standardContractResolver, serializerConfig);
	}
}
