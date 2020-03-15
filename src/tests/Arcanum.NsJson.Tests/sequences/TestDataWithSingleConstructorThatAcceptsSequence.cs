// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using FluentAssertions.Execution;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class TestDataWithSingleConstructorThatAcceptsSequence: TestJsonSerializer {
		[JsonObject(ItemRequired = Required.Always)]
		class Article {
			public String post { get; }
			public LinkedList<String> comments { get; }

			public Article (String post, LinkedList<String> comments) {
				this.post = post;
				this.comments = comments;
			}
		}

		[Fact]
		public void IsDeserialized () {
			var token =
				new JObject {
					["post"] = "This is post.",
					["comments"] =
						new JArray {
							"First comment.",
							"Second comment."
						}
				};
			var article = serializer.FromToken<Article>(token);
			using var assertScope = new AssertionScope();
			article.post.Should().Be("This is post.");
			article.comments.Should().Equal("First comment.", "Second comment.");
		}
	}
}
