// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Net;
	using System.Reflection;

	public static class Module {
		public static IMicroContractResolverBuilder CreateMicroContractResolverBuilder () =>
			new MicroContractResolverBuilder();

		public static IMicroContractResolverBuilder AddStandardContracts (this IMicroContractResolverBuilder builder) =>
			builder
				.AddFactory<BasicJsonContractFactory>()
				.AddFactory<NullableStructJsonContractFactory>()
				.AddFactory<EnumJsonContractFactory>()
				.AddFactory<FlagsEnumJsonContractFactory>()
				.AddFactory<UnionJsonContractFactory>()
				.AddCreator<IPAddress, IpAddressJsonContractCreator>()
				.AddCreator<IPEndPoint, IpEndPointJsonContractCreator>()
				.AddCreator<DnsEndPoint, DnsEndPointJsonContractCreator>()
				.AddMiddlewareFactory<UnionCaseJsonMiddlewareFactory>();

		public static IMicroContractResolverBuilder AddPlatformContracts (this IMicroContractResolverBuilder builder) {
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies) {
				var configurators = assembly.GetCustomAttributes<PlatformJsonContractConfigurator>();
				foreach (var configurator in configurators) configurator.ConfigurePlatformJsonContracts(builder);
			}
			return builder;
		}
	}
}
