// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;
	using System;

	class JsonContractResolver: IContractResolver {
		JsonContractFactory jsonContractFactory { get; }

		MiddlewareJsonContractFactory middlewareJsonContractFactory { get; }

		IContractResolver core { get; }

		JsonContractStorage contractStorage { get; }

		JsonContractStorage middlewareContractStorage { get; }

		public JsonContractResolver (
		IContractResolver core,
		JsonContractFactory jsonContractFactory,
		MiddlewareJsonContractFactory middlewareJsonContractFactory) {
			this.core = core;
			this.jsonContractFactory = jsonContractFactory;
			this.middlewareJsonContractFactory = middlewareJsonContractFactory;
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
			return jsonContractFactory.CreateContract(baseContract);
		}

		JsonContract CreateMiddlewareContract (Type dataType) {
			var contract = contractStorage.GetOrCreate(dataType);
			return middlewareJsonContractFactory.CreateMiddlewareContract(contract);
		}
	}
}
