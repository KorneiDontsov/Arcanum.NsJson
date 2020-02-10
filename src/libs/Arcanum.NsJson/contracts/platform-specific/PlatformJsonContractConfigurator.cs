// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	abstract class PlatformJsonContractConfigurator: Attribute {
		public abstract void ConfigurePlatformJsonContracts (IMicroContractResolverBuilder builder);
	}
}
