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
		///     A value indicating whether there will be a check for additional content after deserializing an
		///     object.
		/// </summary>
		public Boolean checkAdditionalContent { get; }

		/// <summary>
		///     The equality comparer used by the serializer when comparing references.
		/// </summary>
		public IEqualityComparer referenceComparer { get; }

		/// <summary>
		///     A function that creates the <see cref = "IReferenceResolver" /> used by the serializer when
		///     resolving references.
		/// </summary>
		public Func<IReferenceResolver> referenceResolverProvider { get; }

		/// <summary>
		///     The <see cref = "ISerializationBinder" /> used by the serializer when resolving type names.
		/// </summary>
		public ISerializationBinder serializationBinder { get; }

		public JsonSerializerSetup
			(CultureInfo culture,
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

		public JsonSerializerSetup With
			(CultureInfo? culture = null,
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
	}
}
