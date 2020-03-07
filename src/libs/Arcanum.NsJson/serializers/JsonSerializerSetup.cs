// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Immutable;
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

		public IImmutableDictionary<Object, Object> extensions { get; }

		public JsonSerializerSetup
			(CultureInfo culture,
			 UInt32 maxDepth,
			 Boolean checkAdditionalContent,
			 IImmutableDictionary<Object, Object> extensions) {
			this.culture = culture;
			this.maxDepth = maxDepth;
			this.checkAdditionalContent = checkAdditionalContent;
			this.extensions = extensions;
		}

		public JsonSerializerSetup With
			(CultureInfo? culture = null,
			 UInt32? maxDepth = null,
			 Boolean? checkAdditionalContent = null,
			 IImmutableDictionary<Object, Object>? extensions = null) =>
			new JsonSerializerSetup(
				culture ?? this.culture,
				maxDepth ?? this.maxDepth,
				checkAdditionalContent ?? this.checkAdditionalContent,
				extensions ?? this.extensions);

		public JsonSerializerSetup WithExtension (Object extensionId, Object extensionValue) =>
			With(extensions: extensions.SetItem(extensionId, extensionValue));

		public JsonSerializerSetup WithoutExtension (Object extensionId) =>
			With(extensions: extensions.Remove(extensionId));
	}
}
