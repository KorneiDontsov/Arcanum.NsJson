// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.NsTests {
	using FluentAssertions.Execution;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Runtime.Serialization;
	using Xunit;

	public class TestDeserializationCallbacks {
		[JsonConverter(typeof(DataConverter))]
		class Data {
			[OnDeserializing]
			void OnDeserializing (StreamingContext context) {
				var msg = "Not expected 'OnDeserializing' to be invoked because of custom converter.";
				throw new AssertionFailedException(msg);
			}

			[OnDeserialized]
			void OnDeserialized (StreamingContext context) {
				var msg = "Not expected 'OnDeserialized' to be invoked because of custom converter.";
				throw new AssertionFailedException(msg);
			}
		}

		class DataConverter: JsonConverter<Data> {
			/// <inheritdoc />
			public override Boolean CanWrite => false;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Data value, JsonSerializer serializer) =>
				throw new NotSupportedException();

			/// <inheritdoc />
			public override Data ReadJson
				(JsonReader reader,
				 Type objectType,
				 Data existingValue,
				 Boolean hasExistingValue,
				 JsonSerializer serializer) {
				reader.ReadAsString();
				return new Data();
			}
		}

		[Fact]
		public void AreNotInvokedBecauseOfCustomConverter () {
			using var reader = new JTokenReader(new JValue("any-string"));
			_ = new JsonSerializer().Deserialize<Data>(reader);
		}
	}
}
