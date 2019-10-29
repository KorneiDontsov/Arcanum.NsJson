// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractModules {
	using Arcanum.NsJson.ContractResolvers;
	using System.Net;

	public class JsonContractDistributionFactory {
		public static JsonContractDistribution.Builder BuildArcane () =>
			JsonContractDistribution.Build()
				.With<UnionJsonContractModule>()
				.With<EnumJsonContractModule>()
				.With<IPAddress, IpAddressJsonContractModule>()
				.With<DnsEndPoint, DnsEndPointJsonContractModule>();

		public static MiddlewareJsonContractDistribution.Builder BuildArcaneMiddleware () =>
			MiddlewareJsonContractDistribution.Build()
				.With<UnionCaseJsonMiddlewareModule>();
	}
}
