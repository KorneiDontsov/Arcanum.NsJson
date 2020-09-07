// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class TestSetInterface: TestJsonSerializer {
		[Fact]
		public void IsDeserialized () {
			var token = new JArray { "first", "second" };
			var set = serializer.FromToken<ISet<String>>(token);
			set.Should().BeEquivalentTo("first", "second");
		}

		class UnitLikeComparer: IComparer<String> {
			/// <inheritdoc />
			public Int32 Compare (String? x, String? y) => 0;
		}

		[Fact]
		public void IsDeserializedWhereItemsDuplicates () {
			var token = new JArray { "first", "second" };
			var set =
				serializer.FromToken<ISet<String>>(
					token,
					locals =>
						locals.PrepareSamples(
							samples => samples.Self(() => new SortedSet<String>(new UnitLikeComparer()))));
			set.Should().BeEquivalentTo("second");
		}
	}
}
