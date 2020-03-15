// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections;

	public class AotSequenceToJsonConverter: IToJsonConverter {
		public static AotSequenceToJsonConverter shared { get; } = new AotSequenceToJsonConverter();

		/// <inheritdoc />
		public void Write
			(IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) {
			writer.WriteStartArray();
			var enumerator = ((IEnumerable) value).GetEnumerator();
			try {
				while(enumerator.MoveNext()) serializer.Write(writer, enumerator.Current);
			}
			finally {
				if(enumerator is IDisposable disposableEnumerator)
					disposableEnumerator.Dispose();
			}
			writer.WriteEndArray();
		}
	}
}
