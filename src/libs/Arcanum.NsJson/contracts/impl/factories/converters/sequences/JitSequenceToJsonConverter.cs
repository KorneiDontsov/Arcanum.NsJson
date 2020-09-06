// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;

	public class JitSequenceToJsonConverter<T>: IToJsonConverter {
		/// <inheritdoc />
		public void Write (IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) {
			writer.WriteStartArray();
			foreach(var item in (IEnumerable<T>) value) serializer.Write(writer, item);
			writer.WriteEndArray();
		}
	}
}
