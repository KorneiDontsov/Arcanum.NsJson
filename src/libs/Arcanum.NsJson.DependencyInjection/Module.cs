// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.DependencyInjection {
	using Microsoft.Extensions.DependencyInjection;
	using static Arcanum.NsJson.Module;

	public static class Module {
		public static IServiceCollection AddJsonSerializer
			(this IServiceCollection services, IJsonSerializer jsonSerializer) =>
			services.AddSingleton(jsonSerializer);

		public static IServiceCollection AddJsonSerializer (this IServiceCollection services) =>
			services.AddJsonSerializer(standardJsonSerializer);
	}
}
