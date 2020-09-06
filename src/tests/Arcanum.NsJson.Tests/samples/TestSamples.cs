// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.DataContracts;
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using FluentAssertions.Execution;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class TestSamples: TestJsonSerializer {
		public class ParityComparer: IComparer<Int32> {
			/// <inheritdoc />
			public Int32 Compare (Int32 x, Int32 y) =>
				(xIsEven: x % 2 is 0, yIsEven: y % 2 is 0) switch {
					(true, false) => -1,
					(false, true) => 1,
					_ => x.CompareTo(y)
				};
		}

		[Fact]
		public void SelfSampleIsUsed () {
			var token = new JArray { 0, 1, 2, 3, 4, 5, 6, 7 };
			var sortedSet =
				serializer.FromToken<SortedSet<Int32>>(
					token,
					locals =>
						locals.PrepareSamples(
							samples => samples.Self(() => new SortedSet<Int32>(new ParityComparer()))));

			sortedSet.Should().Equal(0, 2, 4, 6, 1, 3, 5, 7);
		}

		[Fact]
		public void ItemSampleIsUsed () {
			var token =
				new JArray {
					new JArray { 0, 1, 2, 3 },
					new JArray { 4, 5, 6, 7 }
				};
			var sortedSets =
				serializer.FromToken<List<SortedSet<Int32>>>(
					token,
					locals =>
						locals.PrepareSamples(
							samples => samples.Item(() => new SortedSet<Int32>(new ParityComparer()))));

			using(new AssertionScope()) {
				sortedSets.Count.Should().Be(2);
				sortedSets[0].Should().Equal(0, 2, 1, 3);
				sortedSets[1].Should().Equal(4, 6, 5, 7);
			}
		}

		class DataWithMemberSamples {
			[JsonRequired]
			public SortedSet<Int32> set { get; }

			public DataWithMemberSamples (SortedSet<Int32> set) =>
				this.set = set;

			[UsedImplicitly]
			public static void PrepareSamples (IDataSampleFactoryBuilder samples) =>
				samples.Member(nameof(set), () => new SortedSet<Int32>(new ParityComparer()));
		}

		[Fact]
		public void MemberSampleIsUsed () {
			var token =
				new JObject {
					["set"] = new JArray { 0, 1, 2, 3, 4, 5, 6, 7 }
				};
			var data = serializer.FromToken<DataWithMemberSamples>(token);

			data.set.Should().Equal(0, 2, 4, 6, 1, 3, 5, 7);
		}
	}
}
