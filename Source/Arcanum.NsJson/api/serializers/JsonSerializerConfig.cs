// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections;
	using System.Globalization;

	public sealed class JsonSerializerConfig {
		public CultureInfo? culture { get; set; }

		public IEqualityComparer? referenceEqualityComparer { get; set; }

		public Func<IReferenceResolver>? referenceResolverProvider { get; set; }

		public ISerializationBinder? serializationBinder { get; set; }

		public ITraceWriter? traceWriter { get; set; }

		public static JsonSerializerConfig @default { get; } = new JsonSerializerConfig();
	}
}
