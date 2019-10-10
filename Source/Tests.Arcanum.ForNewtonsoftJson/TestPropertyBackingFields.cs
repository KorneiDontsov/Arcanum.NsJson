// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Tests.Arcanum.ForNewtonsoftJson {
	using FluentAssertions;
	using FluentAssertions.Execution;
	using FluentAssertions.Json;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using Xunit;

	public sealed class TestPropertyBackingFields: ArcanumJsonContractResolverTest {
		[JsonObject(MemberSerialization.OptIn)]
		sealed class DataWithBackingFields {
			[field: JsonProperty("text", Required = Required.AllowNull)]
			public String? text { get; set; }
		}

		[Fact]
		public void CanBeSerialized () {
			var data = new DataWithBackingFields { text = "some_text" };
			var expectedDataJToken = new JObject { ["text"] = "some_text" };

			var actualDataJToken = JToken.FromObject(data, serializer);

			actualDataJToken.Should().BeEquivalentTo(expectedDataJToken);
		}

		[Fact]
		public void CanBeDeserialized () {
			var dataJToken = new JObject { ["text"] = "some_deserialized_text" };

			using var reader = new JTokenReader(dataJToken);
			var actualData = serializer.Deserialize<DataWithBackingFields>(reader);

			using (new AssertionScope()) {
				actualData.Should().NotBeNull();
				actualData!.text.Should().Be("some_deserialized_text");
			}
		}
	}
}
