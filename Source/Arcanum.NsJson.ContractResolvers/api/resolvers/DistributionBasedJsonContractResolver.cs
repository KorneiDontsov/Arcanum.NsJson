// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class DistributionBasedJsonContractResolver: IContractResolver {
		JsonContractDistribution jsonContractDistribution { get; }

		MiddlewareJsonContractDistribution middlewareJsonContractDistribution { get; }

		IContractResolver core { get; }

		JsonContractStorage contractStorage { get; }

		JsonContractStorage middlewareContractStorage { get; }

		public DistributionBasedJsonContractResolver (
		IContractResolver core,
		JsonContractDistribution jsonContractDistribution,
		MiddlewareJsonContractDistribution middlewareJsonContractDistribution) {
			this.core = core;
			this.jsonContractDistribution = jsonContractDistribution;
			this.middlewareJsonContractDistribution = middlewareJsonContractDistribution;
			contractStorage = new JsonContractStorage(CreateContract);
			middlewareContractStorage = new JsonContractStorage(CreateMiddlewareContract);
		}

		/// <inheritdoc />
		public JsonContract ResolveContract (Type dataType) {
			var chosenContractStorage = JsonContractResolveArgs.Pick().withoutMiddleware
				? contractStorage
				: middlewareContractStorage;
			return chosenContractStorage.GetOrCreate(dataType);
		}

		JsonContract CreateContract (Type dataType) {
			var baseContract = core.ResolveContract(dataType);
			return jsonContractDistribution.CreateContract(baseContract);
		}

		JsonContract CreateMiddlewareContract (Type dataType) {
			var contract = contractStorage.GetOrCreate(dataType);
			return middlewareJsonContractDistribution.CreateMiddlewareContract(contract);
		}
	}
}
