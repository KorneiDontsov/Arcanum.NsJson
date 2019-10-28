// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Abstractions;
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using Xunit;

	public sealed class TestPropertyBackingFields: TestJsonSerializer {
		[JsonObject(MemberSerialization.OptIn)]
		sealed class DataWithBackingFields {
			[field: JsonProperty("text", Required = Required.AllowNull)]
			public String? text { get; set; }
		}

		[Fact]
		public void IsSerialized () {
			var data = new DataWithBackingFields { text = "some_text" };
			var expectedDataJToken = new JObject { ["text"] = "some_text" };

			var actualDataToken = serializer.ToToken(data);

			actualDataToken.Should().BeEquivalentTo(expectedDataJToken);
		}

		[Fact]
		public void IsDeserialized () {
			var dataToken = new JObject { ["text"] = "some_deserialized_text" };

			var actualData = serializer.FromToken<DataWithBackingFields>(dataToken);

			actualData.text.Should().Be("some_deserialized_text");
		}
	}
}
