// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using System;
	using System.Collections.Immutable;

	class LocalSamplesBuilder: IDataSampleFactoryBuilder {
		public Type? dataType { get; }

		CreateSample? createSelfSample;

		CreateSample? createItemSample;

		ImmutableDictionary<String, CreateSample>.Builder? createMemberSamples;

		public LocalSamplesBuilder (Type? dataType = null) =>
			this.dataType = dataType;

		/// <inheritdoc />
		public IDataSampleFactoryBuilder Self<TSample> (Func<TSample> createSample) where TSample: class {
			createSelfSample = new CreateSample(createSample);
			return this;
		}

		/// <inheritdoc />
		public IDataSampleFactoryBuilder Item<TSample> (Func<TSample> createSample) where TSample: class {
			createItemSample = new CreateSample(createSample);
			return this;
		}

		/// <inheritdoc />
		public IDataSampleFactoryBuilder Member<TSample> (String name, Func<TSample> createSample)
			where TSample: class {
			createMemberSamples ??= ImmutableDictionary.CreateBuilder<String, CreateSample>();
			createMemberSamples[name] = new CreateSample(createSample);
			return this;
		}

		public LocalSamples Build () =>
			new LocalSamples(
				dataType,
				createSelfSample: createSelfSample,
				createItemSample: createItemSample,
				createMemberSamples?.ToImmutable());
	}
}
