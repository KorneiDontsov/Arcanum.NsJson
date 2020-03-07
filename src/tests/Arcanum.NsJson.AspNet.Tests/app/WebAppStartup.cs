// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet.Tests {
	using Microsoft.AspNetCore.Builder;
	using Microsoft.Extensions.DependencyInjection;

	public class WebAppStartup {
		public void ConfigureServices (IServiceCollection services) =>
			services.AddControllers().AddJson();

		public void Configure (IApplicationBuilder app) =>
			app
				.UseRouting()
				.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}
