// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	class JsonConverterAdapter: JsonConverter {
		WriteJson writeJson { get; }
		ReadJson readJson { get; }

		public JsonConverterAdapter (WriteJson writeJson, ReadJson readJson) {
			this.writeJson = writeJson;
			this.readJson = readJson;
		}

		/// <inheritdoc />
		public override Boolean CanConvert (Type objectType) => true;

		/// <inheritdoc />
		public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) =>
			writeJson(writer, value, serializer);

		/// <inheritdoc />
		public override Object? ReadJson
		(JsonReader reader,
		 Type objectType,
		 Object? existingValue,
		 JsonSerializer serializer) =>
			readJson(reader, existingValue, serializer);
	}
}
