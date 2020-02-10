// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	public abstract class JsonConverterAdapter: JsonConverter {
		IToJsonConverter? writeConverter { get; }

		IFromJsonConverter? readConverter { get; }

		protected JsonConverterAdapter () {
			writeConverter = this as IToJsonConverter;
			readConverter = this as IFromJsonConverter;
		}

		/// <inheritdoc />
		public override Boolean CanConvert (Type objectType) => true;

		/// <inheritdoc />
		public override sealed Boolean CanWrite => writeConverter is {};

		/// <inheritdoc />
		public override sealed void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) {
			if (value is null) throw new ArgumentNullException(nameof(value));
			else if (writeConverter is null) throw new NotSupportedException();
			else writeConverter.Write(writer, value, serializer);
		}

		/// <inheritdoc />
		public override sealed Boolean CanRead => readConverter is {};

		/// <inheritdoc />
		public override sealed Object? ReadJson
		(JsonReader reader,
		 Type objectType,
		 Object? existingValue,
		 JsonSerializer serializer) =>
			readConverter is null
				? throw new NotSupportedException()
				: readConverter.Read(reader, serializer);
	}
}
