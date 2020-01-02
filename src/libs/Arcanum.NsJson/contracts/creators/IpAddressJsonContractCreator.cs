// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Globalization;
	using System.Net;

	public sealed class IpAddressJsonContractCreator: IJsonContractCreator {
		class IpAddressJsonConverter: JsonConverterAdapter, IToJsonConverter, IFromJsonConverter {
			/// <inheritdoc />
			public void Write (JsonWriter writer, Object value, JsonSerializer serializer) {
				var ipAddressString = value.ToString();
				writer.WriteValue(ipAddressString);
			}

			/// <inheritdoc />
			public Object? Read (JsonReader reader, JsonSerializer serializer) {
				switch (reader.TokenType) {
					case JsonToken.Null: return null;
					case JsonToken.String:
						var ipAddressStr = (String) reader.Value!;
						try {
							return IPAddress.Parse(ipAddressStr);
						}
						catch (FormatException formatEx) {
							var message =
								String.Format(
									CultureInfo.InvariantCulture,
									"Failed to deserialize '{0}' to IPAddress.",
									ipAddressStr);
							throw reader.Exception(message, formatEx);
						}
					case var unexpectedTokenType:
						throw reader.Exception("Expected string but accepted {0}.", unexpectedTokenType);
				}
			}
		}

		/// <inheritdoc />
		public JsonContract CreateContract () =>
			new JsonStringContract(typeof(IPAddress)) { Converter = new IpAddressJsonConverter() };
	}
}
