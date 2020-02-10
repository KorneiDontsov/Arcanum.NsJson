// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet.Tests {
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Mvc.Testing;

	public class WebAppFactory: WebApplicationFactory<WebAppStartup> {
		/// <inheritdoc />
		protected override IWebHostBuilder CreateWebHostBuilder () =>
			WebHost.CreateDefaultBuilder().UseStartup<WebAppStartup>();

		/// <inheritdoc />
		protected override void ConfigureWebHost (IWebHostBuilder builder) {
			builder.UseContentRoot(".");
			base.ConfigureWebHost(builder);
		}
	}
}
