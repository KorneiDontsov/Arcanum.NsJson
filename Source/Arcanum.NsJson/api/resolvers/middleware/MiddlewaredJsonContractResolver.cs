// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using DataContracts;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Concurrent;

	class MiddlewaredJsonContractResolver: JsonContractResolverBase {
		ConcurrentDictionary<Type, JsonContract> middlewareContractDict { get; }

		Func<Type, JsonContract> createMiddlewareContract { get; }

		public MiddlewaredJsonContractResolver () {
			middlewareContractDict = new ConcurrentDictionary<Type, JsonContract>();
			createMiddlewareContract = CreateMiddlewareContract;
		}

		JsonContract ResolveContractWithoutMiddleware (Type type) =>
			base.ResolveContract(type);

		public override sealed JsonContract ResolveContract (Type type) =>
			MiddlewaredContractResolverArgs.Pick().withoutMiddleware
				? ResolveContractWithoutMiddleware(type)
				: middlewareContractDict.GetOrAdd(type, createMiddlewareContract);

		JsonContract CreateMiddlewareContract (Type objectType) {
			var baseContract = ResolveContractWithoutMiddleware(objectType);
			return MaybeMiddlewareConverter(baseContract) is { } middlewareConverter
				? baseContract.WithConverter(middlewareConverter)
				: baseContract;
		}

		JsonConverter? MaybeMiddlewareConverter (JsonContract contract) {
			if (contract.IsOfNonAbstractClass()
			&& dataTypeInfoFactory.Get(contract.UnderlyingType).asUnionCaseInfo is { } unionCaseInfo)
				return CreateUnionCaseMiddlewareContract(contract, unionCaseInfo);
			else
				return null;
		}

		/// <param name = "unionCaseInfo">
		///     Must be of type <see cref = "JsonContract.UnderlyingType" /> specified in
		///     <paramref name = "contract" />.
		/// </param>
		JsonConverter CreateUnionCaseMiddlewareContract (JsonContract contract, IUnionCaseInfo unionCaseInfo) {
			if (contract.UnderlyingType != unionCaseInfo.dataType)
				throw
					new Exception(
						$"'{nameof(unionCaseInfo)}' {unionCaseInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}.");
			else
				return new UnionCaseJsonConverter(unionCaseInfo);
		}
	}
}
