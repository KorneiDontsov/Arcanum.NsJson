// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections;
	using System.Globalization;

	public sealed class JsonSerializerSetup {
		/// <summary>
		///     The culture used when reading JSON.
		/// </summary>
		public CultureInfo culture { get; }

		/// <summary>
		///     The maximum depth allowed when reading JSON. Reading past this depth will throw a
		///     <see cref = "JsonReaderException" />.
		///     <para />
		///     0 value means there is no maximum.
		/// </summary>
		public UInt32 maxDepth { get; }

		/// <summary>
		///     A value indicating whether there will be a check for additional content after deserializing an object.
		/// </summary>
		public Boolean checkAdditionalContent { get; }

		/// <summary>
		///     The equality comparer used by the serializer when comparing references.
		/// </summary>
		public IEqualityComparer referenceComparer { get; }

		/// <summary>
		///     A function that creates the <see cref = "IReferenceResolver" /> used by the serializer when resolving
		///     references.
		/// </summary>
		public Func<IReferenceResolver> referenceResolverProvider { get; }

		/// <summary>
		///     The <see cref = "ISerializationBinder" /> used by the serializer when resolving type names.
		/// </summary>
		public ISerializationBinder serializationBinder { get; }

		public static JsonSerializerSetup arcane { get; } =
			new JsonSerializerSetup(
				CultureInfo.InvariantCulture,
				maxDepth: 32,
				checkAdditionalContent: false,
				referenceComparer: new DefaultReferenceComparer(),
				referenceResolverProvider: () => new JsonSerializer().ReferenceResolver,
				serializationBinder: new JsonSerializer().SerializationBinder);

		public JsonSerializerSetup (
		CultureInfo culture,
		UInt32 maxDepth,
		Boolean checkAdditionalContent,
		IEqualityComparer referenceComparer,
		Func<IReferenceResolver> referenceResolverProvider,
		ISerializationBinder serializationBinder) {
			this.culture = culture;
			this.maxDepth = maxDepth;
			this.checkAdditionalContent = checkAdditionalContent;
			this.referenceComparer = referenceComparer;
			this.referenceResolverProvider = referenceResolverProvider;
			this.serializationBinder = serializationBinder;
		}

		public JsonSerializerSetup With (
		CultureInfo? culture = null,
		UInt32? maxDepth = null,
		Boolean? checkAdditionalContent = null,
		IEqualityComparer? referenceComparer = null,
		Func<IReferenceResolver>? referenceResolverProvider = null,
		ISerializationBinder? serializationBinder = null) =>
			new JsonSerializerSetup(
				culture ?? this.culture,
				maxDepth ?? this.maxDepth,
				checkAdditionalContent ?? this.checkAdditionalContent,
				referenceComparer ?? this.referenceComparer,
				referenceResolverProvider ?? this.referenceResolverProvider,
				serializationBinder ?? this.serializationBinder);

		public void SetUpNsSerializerSettings (JsonSerializerSettings settings, JsonSerializerConfig? config = null) {
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
			settings.Culture = config?.culture ?? culture;
			// constraints
			settings.MaxDepth =
				ConvertDepth(
					config?.maxDepth is var configMaxDepth && configMaxDepth is { }
						? configMaxDepth.GetValueOrDefault()
						: maxDepth);
			settings.CheckAdditionalContent = config?.checkAdditionalContent ?? checkAdditionalContent;
			// reference handling
			settings.EqualityComparer = config?.referenceComparer ?? referenceComparer;
			settings.ReferenceResolverProvider = config?.referenceResolverProvider ?? referenceResolverProvider;
			// type handling
			settings.SerializationBinder = config?.serializationBinder ?? serializationBinder;
			// debug
			settings.TraceWriter = config?.traceWriter;
			//reserved
			settings.Context = default;
			settings.Error = null;
			// ugly and stupid
			settings.Converters = Array.Empty<JsonConverter>();
		}

		public JsonSerializerSettings CreateNsSerializerSettings (
		IContractResolver jsonContractResolver, JsonSerializerConfig? config = null) {
			var settings = new JsonSerializerSettings {
				ContractResolver = jsonContractResolver
			};
			SetUpNsSerializerSettings(settings, config);
			return settings;
		}

		public JsonSerializer CreateNsSerializer (
		IContractResolver jsonContractResolver, JsonSerializerConfig? config = null) {
			var settings = CreateNsSerializerSettings(jsonContractResolver, config);
			return JsonSerializer.Create(settings);
		}
	}
}
