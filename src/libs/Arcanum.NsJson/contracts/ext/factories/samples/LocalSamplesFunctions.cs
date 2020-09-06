// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using System;

	public static class LocalSamplesFunctions {
		static Object localSamplesKey { get; } = new Object();

		public static LocalSamples? MaybeSamples (this ILocalsCollection locals) =>
			(LocalSamples?) locals[localSamplesKey];

		public static void SetSamples (this ILocalsCollection locals, LocalSamples? samples) =>
			locals[localSamplesKey] = samples;

		/// <param name = "overridingSamples">
		///     <see cref = "LocalSamples.dataType" /> of <paramref name = "overridingSamples" /> should be the
		///     same as <see cref = "LocalSamples.dataType" /> of <paramref name = "baseSamples" />.
		/// </param>
		public static LocalSamples Override (this LocalSamples baseSamples, LocalSamples overridingSamples) {
			if(baseSamples.dataType is {}
			   && overridingSamples.dataType is {}
			   && baseSamples.dataType != overridingSamples.dataType) {
				var msg =
					$"Overriding samples has different data type {overridingSamples.dataType} then base samples; "
					+ $"expected {baseSamples.dataType}";
				throw new ArgumentException(msg, nameof(overridingSamples));
			}
			else if(overridingSamples.isEmpty)
				return baseSamples;
			else if(baseSamples.isEmpty)
				return overridingSamples;
			else
				return new LocalSamples(
					baseSamples.dataType ?? overridingSamples.dataType,
					createSelfSample: overridingSamples.createSelfSample ?? baseSamples.createSelfSample,
					createItemSample: overridingSamples.createItemSample ?? baseSamples.createItemSample,
					baseSamples.createMemberSamples.SetItems(overridingSamples.createMemberSamples));
		}

		public static void PrepareSamples (this ILocalsCollection locals, PrepareSamples action) {
			var samplesBuilder = new LocalSamplesBuilder();
			action(samplesBuilder);

			if(samplesBuilder.Build() is {isEmpty: false} newSamples) {
				var targetSamples =
					locals.MaybeSamples() is {dataType: null} actualSamples
						? actualSamples.Override(newSamples)
						: newSamples;
				locals.SetSamples(targetSamples);
			}
		}
	}
}
