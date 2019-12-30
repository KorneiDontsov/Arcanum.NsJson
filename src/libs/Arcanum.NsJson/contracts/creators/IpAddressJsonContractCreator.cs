// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Globalization;
	using System.Net;

	public sealed class IpAddressJsonContractCreator: IJsonContractCreator {
		class IpAddressJsonConverter: JsonConverter<IPAddress> {
			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, IPAddress value, JsonSerializer serializer) {
				var ipAddressString = value.ToString();
				writer.WriteValue(ipAddressString);
			}

			/// <inheritdoc />
			public override IPAddress ReadJson
			(JsonReader reader,
			 Type objectType,
			 IPAddress existingValue,
			 Boolean hasExistingValue,
			 JsonSerializer serializer) {
				reader.CurrentTokenMustBe(JsonToken.String);
				var ipAddressString = (String) reader.Value!;

				try { return IPAddress.Parse(ipAddressString); }
				catch (FormatException formatEx) {
					throw
						reader.Exception(
							String.Format(
								CultureInfo.InvariantCulture,
								"Failed to deserialize '{0}' to IPAddress.",
								ipAddressString),
							formatEx);
				}
			}
		}

		/// <inheritdoc />
		public Type dataType => typeof(IPAddress);

		/// <inheritdoc />
		public JsonContract CreateContract () =>
			new JsonStringContract(dataType) {
				Converter = new IpAddressJsonConverter()
			};
	}
}
