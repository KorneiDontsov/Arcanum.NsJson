// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.ContractModules;
	using Arcanum.NsJson.ContractResolvers;
	using Newtonsoft.Json.Serialization;
	using System.Net;

	static class JsonContractResolverFactory {
		public static IContractResolver @default { get; }

		static JsonContractResolverFactory () {
			var contractResolverCore = new JsonContractResolverCore();
			var jsonContractFactory =
				JsonContractFactory.Build()
					.With<UnionJsonContractModule>()
					.With<EnumJsonContractModule>()
					.With<IPAddress, IpAddressJsonContractModule>()
					.With<DnsEndPoint, DnsEndPointJsonContractModule>()
					.Ok();
			var middlewareJsonContractFactory =
				MiddlewareJsonContractFactory.Build()
					.With<UnionCaseJsonMiddlewareModule>()
					.Ok();
			@default =
				new JsonContractResolver(contractResolverCore, jsonContractFactory, middlewareJsonContractFactory);
		}
	}
}
