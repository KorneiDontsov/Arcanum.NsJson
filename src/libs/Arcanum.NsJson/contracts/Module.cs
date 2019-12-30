// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	public static class Module {
		public static IMicroContractResolverBuilder CreateMicroContractResolverBuilder () =>
			new MicroContractResolverBuilder();

		public static IMicroContractResolverBuilder AddStandardContracts (this IMicroContractResolverBuilder builder) =>
			builder
				.AddCreator<IpAddressJsonContractCreator>()
				.AddCreator<DnsEndPointJsonContractCreator>()
				.AddFactory<NsJsonContractFactory>()
				.AddPatch<EnumJsonContractPatch>()
				.AddPatch<UnionJsonContractPatch>()
				.AddMiddlewarePatch<UnionCaseJsonMiddlewarePatch>();
	}
}
