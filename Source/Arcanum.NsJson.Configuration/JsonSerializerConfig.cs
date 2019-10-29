// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Configuration {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections;
	using System.Globalization;

	public sealed class JsonSerializerConfig {
		/// <summary>
		///     The culture used when reading JSON.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public CultureInfo? culture { get; set; }

		/// <summary>
		///     The maximum depth allowed when reading JSON. Reading past this depth will throw a
		///     <see cref = "JsonReaderException" />.
		///     <para />
		///     0 value means there is no maximum.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public UInt32? maxDepth { get; set; }

		/// <summary>
		///     A value indicating whether there will be a check for additional content after deserializing an object.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public Boolean? checkAdditionalContent { get; set; }

		/// <summary>
		///     The equality comparer used by the serializer when comparing references.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public IEqualityComparer? referenceComparer { get; set; }

		/// <summary>
		///     A function that creates the <see cref = "IReferenceResolver" /> used by the serializer when resolving
		///     references.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public Func<IReferenceResolver>? referenceResolverProvider { get; set; }

		/// <summary>
		///     The <see cref = "ISerializationBinder" /> used by the serializer when resolving type names.
		///     <para />
		///     A null value means that a default value is used.
		/// </summary>
		public ISerializationBinder? serializationBinder { get; set; }

		/// <summary>
		///     The <see cref = "ITraceWriter" /> used by the serializer when writing trace messages.
		///     <para />
		///     A null value means that serializer doesn't writes trace messages.
		/// </summary>
		public ITraceWriter? traceWriter { get; set; }
	}
}
