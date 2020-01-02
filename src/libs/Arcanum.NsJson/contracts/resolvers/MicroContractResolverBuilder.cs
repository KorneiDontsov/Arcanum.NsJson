// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Immutable;

	class MicroContractResolverBuilder: IMicroContractResolverBuilder {
		ImmutableDictionary<Type, IJsonContractCreator>.Builder contractCreators { get; } =
			ImmutableDictionary.CreateBuilder<Type, IJsonContractCreator>();

		ImmutableArray<IJsonContractFactory>.Builder contractFactories { get; } =
			ImmutableArray.CreateBuilder<IJsonContractFactory>();

		ImmutableArray<IJsonContractPatch>.Builder contractPatches { get; } =
			ImmutableArray.CreateBuilder<IJsonContractPatch>();

		ImmutableArray<IJsonMiddlewareFactory>.Builder middlewarePatches { get; } =
			ImmutableArray.CreateBuilder<IJsonMiddlewareFactory>();

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddCreator (Type dataType, IJsonContractCreator contractCreator) {
			contractCreators[dataType] = contractCreator;
			return this;
		}

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddFactory (IJsonContractFactory contractFactory) {
			contractFactories.Insert(0, contractFactory);
			return this;
		}

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddPatch (IJsonContractPatch contractPatch) {
			contractPatches.Add(contractPatch);
			return this;
		}

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddMiddlewareFactory (IJsonMiddlewareFactory middlewareFactory) {
			middlewarePatches.Add(middlewareFactory);
			return this;
		}

		/// <inheritdoc />
		public IContractResolver Build () =>
			new MicroContractResolver(
				contractCreators.ToImmutable(),
				contractFactories.ToImmutable(),
				contractPatches.ToImmutable(),
				middlewarePatches.ToImmutable());
	}
}
