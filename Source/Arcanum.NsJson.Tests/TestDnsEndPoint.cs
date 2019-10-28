// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Abstractions;
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Net;
	using System.Net.Sockets;
	using Xunit;

	public class TestDnsEndPoint: TestJsonSerializer {
		[Theory]
		[InlineData("github.com", 433, AddressFamily.InterNetwork)]
		[InlineData("docs.microsoft.com", 80, AddressFamily.InterNetworkV6)]
		public void IsSerialized (String host, Int32 port, AddressFamily addressFamily) {
			var expectedToken = new JObject {
				["Host"] = host,
				["Port"] = port,
				["AddressFamily"] = addressFamily.ToString()
			};
			var actualToken = serializer.ToToken(new DnsEndPoint(host, port, addressFamily));
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Theory]
		[InlineData("arcanum.any", 80)]
		public void IsSerializedWhereAddressFamilyUnspecified (String host, Int32 port) {
			var expectedToken = new JObject {
				["Host"] = host,
				["Port"] = port
			};
			var actualToken = serializer.ToToken(new DnsEndPoint(host, port, AddressFamily.Unspecified));
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Theory]
		[InlineData("github.com", 433, AddressFamily.InterNetwork)]
		[InlineData("docs.microsoft.com", 80, AddressFamily.InterNetworkV6)]
		public void IsDeserialized (String host, Int32 port, AddressFamily addressFamily) {
			var token = new JObject {
				["Host"] = host,
				["Port"] = port,
				["AddressFamily"] = addressFamily.ToString()
			};
			var actual = serializer.FromToken<DnsEndPoint>(token);
			actual.Host.Should().Be(host);
			actual.Port.Should().Be(port);
			actual.AddressFamily.Should().Be(addressFamily);
		}

		[Theory]
		[InlineData("arcanum.any", 80)]
		public void IsDeserializedWhereAddressFamilyUnspecified (String host, Int32 port) {
			var token = new JObject {
				["Host"] = host,
				["Port"] = port
			};
			var actual = serializer.FromToken<DnsEndPoint>(token);
			actual.Host.Should().Be(host);
			actual.Port.Should().Be(port);
			actual.AddressFamily.Should().Be(AddressFamily.Unspecified);
		}
	}
}
