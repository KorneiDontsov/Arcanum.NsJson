// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Net;
	using Xunit;

	public class TestIpEndPoint: TestJsonSerializer {
		[Fact]
		public void IsSerialized () {
			var ipEndPoint = new IPEndPoint(new IPAddress(new Byte[] { 192, 168, 0, 115 }), 433);
			var expected =
				new JObject {
					["Address"] = "192.168.0.115",
					["Port"] = 433
				};
			var actual = serializer.ToToken(ipEndPoint);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void IsDeserialized () {
			var token =
				new JObject {
					["Address"] = "239.205.178.127",
					["Port"] = 80
				};
			var expected = new IPEndPoint(new IPAddress(new Byte[] { 239, 205, 178, 127 }), 80);
			var actual = serializer.FromToken<IPEndPoint>(token);
			actual.Should().Be(expected);
		}
	}
}
