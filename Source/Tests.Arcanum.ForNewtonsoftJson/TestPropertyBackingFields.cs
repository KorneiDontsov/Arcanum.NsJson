// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace Tests.Arcanum.ForNewtonsoftJson
{
	public sealed class TestPropertyBackingFields : ArcanumJsonContractResolverTest
	{
		[JsonObject(MemberSerialization.OptIn)]
		private sealed class DataWithBackingFields
		{
			[field: JsonProperty("text", Required = Required.AllowNull)]
			public String? text { get; set; }
		}

		[Fact]
		public void CanBeSerialized ()
		{
			var data = new DataWithBackingFields { text = "some_text" };
			var expectedDataJToken = new JObject { ["text"] = "some_text" };

			var actualDataJToken = JToken.FromObject(data, serializer);

			_ = actualDataJToken.Should().BeEquivalentTo(expectedDataJToken);
		}

		[Fact]
		public void CanBeDeserialized ()
		{
			var dataJToken = new JObject { ["text"] = "some_deserialized_text" };

			DataWithBackingFields? actualData;
			using (var reader = new JTokenReader(dataJToken))
			{
				actualData = serializer.Deserialize<DataWithBackingFields>(reader);
			}

			using (new AssertionScope())
			{
				_ = actualData.Should().NotBeNull();
				_ = actualData!.text.Should().Be("some_deserialized_text");
			}
		}
	}
}
