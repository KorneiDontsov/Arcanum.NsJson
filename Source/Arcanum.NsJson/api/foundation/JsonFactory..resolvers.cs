// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;

	partial class JsonFactory {
		static JsonContractResolverCore contractResolverCore { get; } =
			new JsonContractResolverCore();

		public static IContractResolver defaultContractResolver { get; } = ContractResolver();

		static IContractResolver ContractResolver () {
			var jsonContractFactory =
				JsonContractFactory.Build()
					.With<UnionJsonContractModule>()
					.Ok();
			var middlewareJsonContractFactory =
				MiddlewareJsonContractFactory.Build()
					.With<UnionCaseJsonMiddlewareModule>()
					.Ok();
			return new JsonContractResolver(contractResolverCore, jsonContractFactory, middlewareJsonContractFactory);
		}
	}
}
