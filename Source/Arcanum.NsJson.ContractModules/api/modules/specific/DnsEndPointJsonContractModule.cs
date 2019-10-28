// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractModules {
	using Arcanum.NsJson.ContractResolvers;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Net;
	using System.Net.Sockets;
	using static JsonContractKit<System.Net.DnsEndPoint>;

	public sealed class DnsEndPointJsonContractModule: IJsonContractModule {
		/// <inheritdoc />
		public JsonContract CreateContract (JsonContract baseContract) {
			var hostProp = ReadOnlyProperty(nameof(DnsEndPoint.Host), dnsEndPoint => dnsEndPoint.Host);
			var portProp = ReadOnlyProperty(nameof(DnsEndPoint.Port), dnsEndPoint => dnsEndPoint.Port);
			var addressFamilyProp =
				ReadOnlyProperty(
					nameof(DnsEndPoint.AddressFamily),
					dnsEndPoint => dnsEndPoint.AddressFamily,
					Required.Default,
					DefaultValueHandling.IgnoreAndPopulate,
					defaultValue: AddressFamily.Unspecified);

			return new JsonObjectContract(typeof(DnsEndPoint)) {
				CreatedType = typeof(DnsEndPoint),
				MemberSerialization = MemberSerialization.OptOut,
				Properties = { hostProp, portProp, addressFamilyProp },
				CreatorParameters = { hostProp, portProp, addressFamilyProp },
				OverrideCreator =
					OverrideCreator(
						(String host, Int32 port, AddressFamily addressFamily) =>
							new DnsEndPoint(host, port, addressFamily))
			};
		}
	}
}
