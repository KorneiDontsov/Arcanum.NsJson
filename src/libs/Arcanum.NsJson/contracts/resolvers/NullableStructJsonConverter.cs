// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	class NullableStructJsonConverter: JsonConverter {
		Type notNullDataType { get; }

		public NullableStructJsonConverter (Type notNullDataType) => this.notNullDataType = notNullDataType;

		/// <inheritdoc />
		public override Boolean CanConvert (Type objectType) => true;

		/// <inheritdoc />
		public override Object? ReadJson
		(JsonReader reader,
		 Type objectType,
		 Object? existingValue,
		 JsonSerializer serializer) =>
			reader.TokenType is JsonToken.Null
				? null
				: serializer.Deserialize(reader, notNullDataType);

		/// <inheritdoc />
		public override Boolean CanWrite => false;

		/// <inheritdoc />
		public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) =>
			throw new NotSupportedException();
	}
}
