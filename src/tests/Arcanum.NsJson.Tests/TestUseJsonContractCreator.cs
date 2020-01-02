// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using FluentAssertions.Execution;
	using FluentAssertions.Json;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json.Serialization;
	using System;
	using Xunit;

	public class TestUseJsonContractCreator: TestJsonSerializer {
		[MutDnsEndPointJsonContractCreator]
		class MutDnsEndPoint {
			public String? maybeHost { get; set; }

			public Int32 maybePort { get; set; }
		}

		class MutDnsEndPointJsonContractCreator: Attribute, IJsonContractCreator {
			/// <inheritdoc />
			public JsonContract CreateContract () {
				var hostProp =
					new JsonProperty {
						PropertyName = "Host",
						UnderlyingName = nameof(MutDnsEndPoint.maybeHost),
						PropertyType = typeof(String),
						DeclaringType = typeof(MutDnsEndPoint),
						Readable = true,
						Writable = true,
						ValueProvider =
							new ValueProvider<MutDnsEndPoint, String?>(d => d.maybeHost, (d, v) => d.maybeHost = v),
						Required = Required.Default,
						DefaultValueHandling = DefaultValueHandling.Ignore
					};
				var portProp =
					new JsonProperty {
						PropertyName = "Port",
						UnderlyingName = nameof(MutDnsEndPoint.maybePort),
						PropertyType = typeof(Int32),
						DeclaringType = typeof(MutDnsEndPoint),
						Readable = true,
						Writable = true,
						ValueProvider =
							new ValueProvider<MutDnsEndPoint, Int32>(d => d.maybePort, (d, v) => d.maybePort = v),
						Required = Required.Default,
						DefaultValueHandling = DefaultValueHandling.Ignore,
						DefaultValue = 0
					};
				return
					new JsonObjectContract(typeof(MutDnsEndPoint)) {
						MemberSerialization = MemberSerialization.OptOut,
						Properties = { hostProp, portProp },
						DefaultCreator = () => new MutDnsEndPoint()
					};
			}
		}

		[Fact]
		public void TargetIsSerialized () {
			var data = new MutDnsEndPoint { maybeHost = "arcanum.com", maybePort = 0 };
			var expected = new JObject { ["Host"] = "arcanum.com" };
			var actual = serializer.ToToken(data);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void TargetIsDeserialized () {
			var token = new JObject { ["Port"] = 433 };
			var data = serializer.FromToken<MutDnsEndPoint>(token);

			using var assertionScope = new AssertionScope();
			data.maybeHost.Should().BeNull();
			data.maybePort.Should().Be(433);
		}
	}
}
