// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

[assembly: Arcanum.NsJson.Ext.AspNet.MvcJsonContractConfigurator]

namespace Arcanum.NsJson.Ext.AspNet {
	using Arcanum.NsJson.Contracts;

	class MvcJsonContractConfigurator: PlatformJsonContractConfigurator {
		/// <inheritdoc />
		public override void ConfigurePlatformJsonContracts (IMicroContractResolverBuilder builder) =>
			builder.AddContractFactory(new ProblemDetailsJsonContractFactory());
	}
}
