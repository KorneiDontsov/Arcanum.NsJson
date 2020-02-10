// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.DependencyInjection.Tests {
	using Microsoft.Extensions.DependencyInjection;
	using Xunit;

	public class TestModule {
		[Fact]
		public void AddsJsonSerializer () {
			var serviceProvider =
				new ServiceCollection()
					.AddJsonSerializer()
					.BuildServiceProvider();
			_ = serviceProvider.GetRequiredService<IJsonSerializer>();
		}
	}
}
