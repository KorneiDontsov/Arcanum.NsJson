// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet.Tests {
	using FluentAssertions.Json;
	using Newtonsoft.Json.Linq;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;
	using static Arcanum.NsJson.AspNet.Tests.HttpContentFunctions;

	public class TestMvcJson: IClassFixture<WebAppFactory> {
		WebAppFactory webApp { get; }

		public TestMvcJson (WebAppFactory webApp) => this.webApp = webApp;

		[Fact]
		public async Task IsWorked () {
			var requestJson =
				new JObject {
					["author"] = "Kornei Dontsov",
					["message"] = "Hello, tests!"
				};
			var expectedResponseJson =
				new JObject {
					["author"] = "Kornei Dontsov",
					["message"] = "Hello, tests!",
					["kind"] = "Test"
				};

			var client = webApp.CreateClient();
			using var request =
				new HttpRequestMessage(HttpMethod.Post, "/test_messages") { Content = JsonContent(requestJson) };
			using var response = await client.SendAsync(request);

			response.EnsureSuccessStatusCode();
			var actualResponseJson = await response.Content.ReadAsJTokenAsync();
			actualResponseJson.Should().BeEquivalentTo(expectedResponseJson);
		}
	}
}
