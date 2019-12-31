// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	class MiddlewareJsonConverter: JsonConverterAdapter, IWriteJsonConverter, IReadJsonConverter {
		WriteJson writeJson { get; }
		ReadJson readJson { get; }

		public MiddlewareJsonConverter (WriteJson writeJson, ReadJson readJson) {
			this.writeJson = writeJson;
			this.readJson = readJson;
		}

		/// <inheritdoc />
		public void Write (JsonWriter writer, Object value, JsonSerializer serializer) =>
			writeJson(writer, value, serializer);

		/// <inheritdoc />
		public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) =>
			readJson(reader, serializer);
	}
}
