// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class NullableStructJsonContractFactory: IJsonContractFactory {
		class NullableStructJsonConverter: JsonConverterAdapter, IReadJsonConverter {
			Type notNullDataType { get; }
			public NullableStructJsonConverter (Type notNullDataType) => this.notNullDataType = notNullDataType;

			/// <inheritdoc />
			public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) =>
				reader.TokenType is JsonToken.Null
					? null
					: serializer.Deserialize(reader, notNullDataType);
		}

		/// <inheritdoc />
		public JsonContract? MayCreateContract (Type dataType) {
			if (Nullable.GetUnderlyingType(dataType) is {} notNullDataType) {
				var converter = new NullableStructJsonConverter(notNullDataType);
				return new JsonLinqContract(dataType) { Converter = converter };
			}
			else return null;
		}
	}
}
