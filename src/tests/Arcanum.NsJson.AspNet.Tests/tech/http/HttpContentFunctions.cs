// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet.Tests {
	using Newtonsoft.Json.Linq;
	using System.Net.Http;
	using System.Net.Mime;
	using System.Text;
	using System.Threading.Tasks;

	static class HttpContentFunctions {
		public static StringContent JsonContent (JToken token) =>
			new StringContent(token.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json);

		public static async Task<JToken> ReadAsJTokenAsync (this HttpContent httpContent) {
			var json = await httpContent.ReadAsStringAsync();
			return JToken.Parse(json);
		}
	}
}
