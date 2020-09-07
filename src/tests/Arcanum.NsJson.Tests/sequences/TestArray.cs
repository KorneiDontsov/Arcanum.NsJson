// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using Newtonsoft.Json.Linq;
	using System;
	using Xunit;

	public class TestArray: TestJsonSerializer {
		[Fact]
		public void IsSerialized () {
			var collection = new[] { "first", "second" };
			var expected = new JArray { "first", "second" };
			var actual = serializer.ToToken(collection);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void IsDeserialized () {
			var token = new JArray { "first", "second" };
			var collection = serializer.FromToken<String[]>(token);
			collection.Should().Equal("first", "second");
		}
	}
}
