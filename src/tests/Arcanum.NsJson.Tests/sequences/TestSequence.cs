// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Xunit;
	using static FluentAssertions.FluentActions;

	public class TestSequence: TestJsonSerializer {
		class Sequence: IEnumerable<String> {
			/// <inheritdoc />
			public IEnumerator<String> GetEnumerator () {
				yield return "first";
				yield return "second";
			}

			/// <inheritdoc />
			IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
		}

		readonly Sequence sequence = new Sequence();

		[Fact]
		public void IsSerialized () {
			var expected = new JArray { "first", "second" };
			var actual = serializer.ToToken(sequence);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void MayNotBeDeserialized () {
			void Action () {
				var token = new JArray { "first", "second" };
				_ = serializer.FromToken<Sequence>(token);
			}

			Invoking(Action).Should().Throw<JsonSerializationException>();
		}
	}
}
