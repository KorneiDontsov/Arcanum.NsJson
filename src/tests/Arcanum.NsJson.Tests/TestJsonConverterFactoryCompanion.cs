// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using FluentAssertions.Json;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using Xunit;

	public class TestJsonConverterFactoryCompanion: TestJsonSerializer {
		class UnitJsonDeserializationException: JsonSerializationException {
			public UnitJsonDeserializationException (String message): base(message) { }
		}

		readonly struct Unit {
			class JsonConverter: IJsonConverter {
				/// <inheritdoc />
				public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
					reader.CurrentTokenMustBe(JsonToken.String);
					var value = (String) reader.Value!;
					return value is "unit"
						? (Unit) default
						: throw new UnitJsonDeserializationException($"Expected 'unit' but accepted {value}.");
				}

				/// <inheritdoc />
				public void Write
					(IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) =>
					writer.WriteValue("unit");
			}

			[UsedImplicitly]
			public class Companion: IJsonConverterFactory {
				/// <inheritdoc />
				public void Handle (IJsonConverterRequest request) =>
					request.Return(new JsonConverter());
			}
		}

		[Fact]
		public void TargetIsSerialized () {
			var expected = new JValue("unit");
			var actual = serializer.ToToken(new Unit());
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void TargetIsDeserialized () {
			var token = new JValue("unit");
			var actual = serializer.MayFromToken<Unit>(token);
			actual.Should().NotBeNull();
		}

		[Fact]
		public void TargetIsNotDeserializedBecauseOfFailure () {
			Action action = () => serializer.MayFromToken<Unit?>(new JValue("not unit"));
			action.Should().ThrowExactly<UnitJsonDeserializationException>();
		}
	}
}
