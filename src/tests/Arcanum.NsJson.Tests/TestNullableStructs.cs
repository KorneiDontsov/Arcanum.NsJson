// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using Xunit;

	public class TestNullableStructs: TestJsonSerializer {
		[JsonObject(ItemRequired = Required.Always)]
		readonly struct Point: IEquatable<Point> {
			public Int64 x { get; }
			public Int64 y { get; }

			public Point (Int64 x, Int64 y) {
				this.x = x;
				this.y = y;
			}

			/// <inheritdoc />
			public Boolean Equals (Point other) => x == other.x && y == other.y;

			/// <inheritdoc />
			public override Boolean Equals (Object? obj) => obj is Point other && Equals(other);

			/// <inheritdoc />
			public override Int32 GetHashCode () => HashCode.Combine(x, y);
		}

		[Fact]
		public void JsonObjectIsDeserializedToValueOfNullableStruct () {
			var token =
				new JObject {
					["x"] = 44,
					["y"] = 13
				};
			Point? expected = new Point(44, 13);
			var actual = serializer.MayFromToken<Point?>(token);
			actual.Should().Be(expected);
		}

		[Fact]
		public void JsonNullIsDeserializedToNullOfNullableStruct () {
			var token = new JValue((Object?) null);
			var actual = serializer.MayFromToken<Point?>(token);
			actual.Should().BeNull();
		}

		[JsonConverter(typeof(UnitJsonConverter))]
		readonly struct Unit { }

		class UnitJsonDeserializationException: JsonSerializationException {
			public UnitJsonDeserializationException (String message): base(message) { }
		}

		class UnitJsonConverter: JsonConverter<Unit> {
			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Unit value, JsonSerializer serializer) =>
				writer.WriteValue("unit");

			/// <inheritdoc />
			public override Unit ReadJson
				(JsonReader reader,
				 Type objectType,
				 Unit existingValue,
				 Boolean hasExistingValue,
				 JsonSerializer serializer) {
				reader.CurrentTokenMustBe(JsonToken.String);
				var value = (String) reader.Value!;
				return value is "unit"
					? (Unit) default
					: throw new UnitJsonDeserializationException($"Expected 'unit' but accepted {value}.");
			}
		}

		[Fact]
		public void ConverterIsUsed () {
			Action action = () => serializer.MayFromToken<Unit?>(new JValue("not unit"));
			action.Should().ThrowExactly<UnitJsonDeserializationException>();
		}

		[Fact]
		public void JsonObjectIsDeserializedToValueOfNullableStructWithConverter () {
			var token = new JValue("unit");
			var actual = serializer.MayFromToken<Unit?>(token);
			actual.Should().NotBeNull();
		}

		[Fact]
		public void JsonNullIsDeserializedToNullOfNullableStructWithConverterWithoutUsingConverter () {
			var token = new JValue((Object?) null);
			var actual = serializer.MayFromToken<Unit?>(token);
			actual.Should().BeNull();
		}
	}
}
