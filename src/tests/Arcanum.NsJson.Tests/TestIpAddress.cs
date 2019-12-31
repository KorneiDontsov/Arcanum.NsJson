// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Xunit;

	public class TestIpAddress: TestJsonSerializer {
		public static IEnumerable<String> EnumerateIpAddressStrings () {
			yield return "127.0.0.1";
			yield return "192.168.0.100";
			yield return "74.162.98.212";
			yield return "FE80:CD00:0000:0CDE:1257:0000:211E:729C";
			yield return "FE80:CD00:0:CDE:1257:0:211E:729C";
		}

		public static TheoryData<String> ipAddressStrings =>
			EnumerateIpAddressStrings().ToTheoryData();

		public static TheoryData<IPAddress> ipAddresses =>
			EnumerateIpAddressStrings().Select(IPAddress.Parse).ToTheoryData();

		[Theory, MemberData(nameof(ipAddresses))]
		public void IsSerialized (IPAddress ipAddress) {
			var expectedToken = new JValue(ipAddress.ToString());
			var actualToken = serializer.ToToken(ipAddress);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Theory, MemberData(nameof(ipAddressStrings))]
		public void IsDeserialized (String ipAddressString) {
			var token = new JValue(ipAddressString);
			var expected = IPAddress.Parse(ipAddressString);

			var actual = serializer.FromToken<IPAddress>(token);

			actual.Should().Be(expected);
		}

		[Fact]
		public void NullIsDeserialized () {
			var token = new JValue((Object?) null);
			var actual = serializer.MayFromToken<IPAddress?>(token);
			actual.Should().BeNull();
		}
	}
}
