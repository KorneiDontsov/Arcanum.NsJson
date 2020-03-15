// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class TestCollectionInterface: TestJsonSerializer {
		[Fact]
		public void IsDeserialized () {
			var token = new JArray { "first", "second" };
			var collection = serializer.FromToken<ICollection<String>>(token);
			collection.Should().Equal("first", "second");
		}
	}
}
