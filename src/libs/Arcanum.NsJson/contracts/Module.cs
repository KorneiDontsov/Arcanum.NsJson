// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System.Net;

	public static class Module {
		public static IMicroContractResolverBuilder CreateMicroContractResolverBuilder () =>
			new MicroContractResolverBuilder();

		public static IMicroContractResolverBuilder AddStandardContracts (this IMicroContractResolverBuilder builder) =>
			builder
				.AddFactory<BasicJsonContractFactory>()
				.AddFactory<NullableStructJsonContractFactory>()
				.AddFactory<EnumJsonContractFactory>()
				.AddFactory<UnionJsonContractFactory>()
				.AddCreator<IPAddress, IpAddressJsonContractCreator>()
				.AddCreator<DnsEndPoint, DnsEndPointJsonContractCreator>()
				.AddMiddlewareFactory<UnionCaseJsonMiddlewareFactory>();
	}
}
