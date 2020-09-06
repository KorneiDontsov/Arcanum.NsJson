// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System.Collections.Immutable;

	class MicroContractResolverBuilder: IMicroContractResolverBuilder {
		ImmutableArray<IJsonContractFactory>.Builder contractFactories { get; } =
			ImmutableArray.CreateBuilder<IJsonContractFactory>();

		ImmutableArray<IJsonMiddlewareFactory>.Builder middlewareFactories { get; } =
			ImmutableArray.CreateBuilder<IJsonMiddlewareFactory>();

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddContractFactory (IJsonContractFactory contractFactory) {
			contractFactories.Add(contractFactory);
			return this;
		}

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddConverterFactory (IJsonConverterFactory converterFactory) {
			contractFactories.Add(new JsonConverterFactoryAdapter(converterFactory));
			return this;
		}

		/// <inheritdoc />
		public IMicroContractResolverBuilder AddMiddlewareFactory (IJsonMiddlewareFactory middlewareFactory) {
			middlewareFactories.Add(middlewareFactory);
			return this;
		}

		/// <inheritdoc />
		public IContractResolver Build () {
			contractFactories.Reverse();
			var immContractFactories = contractFactories.ToImmutable();
			contractFactories.Reverse();

			var immMiddlewareFactories = middlewareFactories.ToImmutable();

			return new MicroContractResolver(immContractFactories, immMiddlewareFactories);
		}
	}
}
