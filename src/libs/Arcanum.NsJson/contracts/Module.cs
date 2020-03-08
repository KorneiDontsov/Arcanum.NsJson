// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Reflection;

	public static class Module {
		public static IMicroContractResolverBuilder CreateMicroContractResolverBuilder () =>
			new MicroContractResolverBuilder();

		public static IMicroContractResolverBuilder AddStandardContracts (this IMicroContractResolverBuilder builder) =>
			builder
				.AddContractFactory(new NsJsonBasedContractFactory())
				.AddContractFactory(new FlagsEnumJsonContractFactory())
				.AddConverterFactory(new EnumJsonConverterFactory())
				.AddConverterFactory(new UnionJsonConverterFactory())
				.AddConverterFactory(new IpAddressJsonConverterFactory())
				.AddContractFactory(new IpEndPointJsonContractFactory())
				.AddContractFactory(new DnsEndPointJsonContractFactory())
				.AddMiddlewareFactory(new SerializeCallbacksJsonMiddlewareFactory())
				.AddMiddlewareFactory(new UnionCaseJsonMiddlewareFactory())
				.AddMiddlewareFactory(new DeserializeCallbacksJsonMiddlewareFactory());

		public static IMicroContractResolverBuilder AddPlatformContracts (this IMicroContractResolverBuilder builder) {
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies) {
				var configurators = assembly.GetCustomAttributes<PlatformJsonContractConfigurator>();
				foreach (var configurator in configurators) configurator.ConfigurePlatformJsonContracts(builder);
			}
			return builder;
		}

		public static IContractResolver standardJsonContractResolver { get; } =
			CreateMicroContractResolverBuilder()
				.AddStandardContracts()
				.AddPlatformContracts()
				.Build();
	}
}
