// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Xunit;

	public class TestCollection: TestJsonSerializer {
		class Collection: ICollection<String> {
			List<String> list { get; } = new List<String>();

			/// <inheritdoc />
			public IEnumerator<String> GetEnumerator () => list.GetEnumerator();

			/// <inheritdoc />
			IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable) list).GetEnumerator();

			/// <inheritdoc />
			public void Add (String item) => list.Add(item);

			/// <inheritdoc />
			public void Clear () => list.Clear();

			/// <inheritdoc />
			public Boolean Contains (String item) => list.Contains(item);

			/// <inheritdoc />
			public void CopyTo (String[] array, Int32 arrayIndex) => list.CopyTo(array, arrayIndex);

			/// <inheritdoc />
			public Boolean Remove (String item) => list.Remove(item);

			/// <inheritdoc />
			public Int32 Count => list.Count;

			/// <inheritdoc />
			public Boolean IsReadOnly => false;
		}

		[Fact]
		public void IsSerialized () {
			var collection = new Collection { "first", "second" };
			var expected = new JArray { "first", "second" };
			var actual = serializer.ToToken(collection);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void IsDeserialized () {
			var token = new JArray { "first", "second" };
			var collection = serializer.FromToken<Collection>(token);
			collection.Should().Equal("first", "second");
		}
	}
}
