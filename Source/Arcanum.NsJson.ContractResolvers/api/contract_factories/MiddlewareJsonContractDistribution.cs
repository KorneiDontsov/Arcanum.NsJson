// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json.Serialization;
	using System.Collections.Immutable;

	public sealed class MiddlewareJsonContractDistribution {
		ImmutableList<IJsonMiddlewareModule> modules { get; }

		MiddlewareJsonContractDistribution (ImmutableList<IJsonMiddlewareModule> modules) =>
			this.modules = modules;

		public JsonContract CreateMiddlewareContract (JsonContract contract) {
			foreach (var module in modules)
				if (module.MayCreateConverter(contract) is { } middlewareJsonConverter)
					return contract.WithConverter(middlewareJsonConverter);

			return contract;
		}

		#region builder
		public static Builder Build () => new Builder();

		public sealed class Builder {
			ImmutableList<IJsonMiddlewareModule>.Builder modules { get; }

			public Builder () =>
				modules = ImmutableList.CreateBuilder<IJsonMiddlewareModule>();

			public Builder With (IJsonMiddlewareModule module) {
				modules.Add(module);
				return this;
			}

			public Builder With<TModule> () where TModule: class, IJsonMiddlewareModule, new() =>
				With(new TModule());

			public MiddlewareJsonContractDistribution Ok () =>
				new MiddlewareJsonContractDistribution(modules: modules.ToImmutable());
		}
		#endregion
	}
}
