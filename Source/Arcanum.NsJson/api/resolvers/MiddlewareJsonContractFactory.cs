// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;
	using System.Collections.Immutable;

	class MiddlewareJsonContractFactory {
		ImmutableList<IJsonMiddlewareModule> modules { get; }

		MiddlewareJsonContractFactory (ImmutableList<IJsonMiddlewareModule> modules) =>
			this.modules = modules;

		public JsonContract CreateMiddlewareContract (JsonContract contract) {
			foreach (var module in modules)
				if (module.MayCreateConverter(contract) is { } middlewareJsonConverter)
					return contract.WithConverter(middlewareJsonConverter);

			return contract;
		}

		#region builder
		public static Builder Build () => new Builder();

		public class Builder {
			ImmutableList<IJsonMiddlewareModule>.Builder modules { get; }

			public Builder () =>
				modules = ImmutableList.CreateBuilder<IJsonMiddlewareModule>();

			public Builder With (IJsonMiddlewareModule module) {
				modules.Add(module);
				return this;
			}

			public Builder With<TModule> () where TModule: class, IJsonMiddlewareModule, new() =>
				With(new TModule());

			public MiddlewareJsonContractFactory Ok () =>
				new MiddlewareJsonContractFactory(modules: modules.ToImmutable());
		}
		#endregion
	}
}
