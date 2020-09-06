// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Immutable;

	public sealed class LocalSamples {
		/// <summary>
		///     Type of data which samples there are.
		///     If null then samples correspond to the data then is currently being deserialized.
		/// </summary>
		public Type? dataType { get; }

		public CreateSample? createSelfSample { get; }

		public CreateSample? createItemSample { get; }

		public ImmutableDictionary<String, CreateSample> createMemberSamples { get; }

		public LocalSamples
			(Type? dataType = null,
			 CreateSample? createSelfSample = null,
			 CreateSample? createItemSample = null,
			 ImmutableDictionary<String, CreateSample>? createMemberSamples = null) {
			this.dataType = dataType;
			this.createSelfSample = createSelfSample;
			this.createItemSample = createItemSample;
			this.createMemberSamples = createMemberSamples ?? ImmutableDictionary<String, CreateSample>.Empty;
		}

		public Boolean isEmpty =>
			createSelfSample is null && createItemSample is null && createMemberSamples.IsEmpty;
	}
}
