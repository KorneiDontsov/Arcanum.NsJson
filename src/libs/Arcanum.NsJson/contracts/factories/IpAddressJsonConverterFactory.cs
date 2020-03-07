// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Globalization;
	using System.Net;

	public sealed class IpAddressJsonConverterFactory: IJsonConverterFactory {
		class IpAddressJsonConverter: IJsonConverter {
			/// <inheritdoc />
			public void Write (IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) {
				var ipAddressString = value.ToString();
				writer.WriteValue(ipAddressString);
			}

			/// <inheritdoc />
			public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
				if (reader.TokenType is JsonToken.String) {
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
				}
				else
					throw reader.Exception("Expected string but accepted {0}.", reader.TokenType);
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request) {
			if (request.dataType == typeof(IPAddress)) request.Return(new IpAddressJsonConverter());
		}
	}
}
