// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

static class TheoryDataFactory {
	public static TheoryData<T> ToTheoryData<T> (this IEnumerable<T> source) =>
		source.Aggregate(
			seed: new TheoryData<T>(),
			(theoryData, item) => {
				theoryData.Add(item);
				return theoryData;
			});
}
