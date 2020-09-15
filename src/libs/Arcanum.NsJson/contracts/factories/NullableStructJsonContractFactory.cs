// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	public sealed class NullableStructJsonContractFactory: IJsonContractFactory {
		class NullableStructJsonConverter: JsonConverterAdapter, IFromJsonConverter {
			Type notNullDataType { get; }
			public NullableStructJsonConverter (Type notNullDataType) => this.notNullDataType = notNullDataType;

			/// <inheritdoc />
			public Object? Read (JsonReader reader, JsonSerializer serializer) =>
				reader.TokenType is JsonToken.Null
					? null
					: serializer.Deserialize(reader, notNullDataType);
		}

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if (Nullable.GetUnderlyingType(request.dataType) is {} notNullDataType) {
				var contract = BasicJsonContractFactory.CreateContract(request.dataType);
				contract.Converter = new NullableStructJsonConverter(notNullDataType);
				request.Return(contract);
			}
		}
	}
}
