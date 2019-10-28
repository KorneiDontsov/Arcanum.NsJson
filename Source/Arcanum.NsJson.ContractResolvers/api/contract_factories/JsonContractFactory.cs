// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Immutable;

	public sealed class JsonContractFactory {
		ImmutableDictionary<Type, IJsonContractModule> modules { get; }

		ImmutableList<IJsonContractGenericModule> genericModules { get; }

		JsonContractFactory (
		ImmutableDictionary<Type, IJsonContractModule> modules,
		ImmutableList<IJsonContractGenericModule> genericModules) {
			this.modules = modules;
			this.genericModules = genericModules;
		}

		public JsonContract CreateContract (JsonContract baseContract) {
			if (modules.TryGetValue(baseContract.UnderlyingType, out var module))
				return module.CreateContract(baseContract);
			else {
				foreach (var genericModule in genericModules)
					if (genericModule.MayCreateContract(baseContract) is { } contract)
						return contract;

				return baseContract;
			}
		}

		#region builder
		public static Builder Build () => new Builder();

		public sealed class Builder {
			ImmutableDictionary<Type, IJsonContractModule>.Builder modules { get; }

			ImmutableList<IJsonContractGenericModule>.Builder genericModules { get; }

			internal Builder () {
				modules = ImmutableDictionary.CreateBuilder<Type, IJsonContractModule>();
				genericModules = ImmutableList.CreateBuilder<IJsonContractGenericModule>();
			}

			public Builder With (Type dataType, IJsonContractModule module) {
				modules.Add(dataType, module);
				return this;
			}

			public Builder With<T, TModule> () where TModule: class, IJsonContractModule, new() =>
				With(typeof(T), new TModule());

			public Builder With (IJsonContractGenericModule genericModule) {
				genericModules.Add(genericModule);
				return this;
			}

			public Builder With<TModule> () where TModule: class, IJsonContractGenericModule, new() =>
				With(new TModule());

			public JsonContractFactory Ok () =>
				new JsonContractFactory(
					modules: modules.ToImmutable(),
					genericModules: genericModules.ToImmutable());
		}
		#endregion
	}
}
