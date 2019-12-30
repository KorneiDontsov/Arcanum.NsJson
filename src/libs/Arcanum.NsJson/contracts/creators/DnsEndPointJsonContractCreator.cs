// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Net;
	using System.Net.Sockets;

	public sealed class DnsEndPointJsonContractCreator: IJsonContractCreator {
		/// <inheritdoc />
		public Type dataType => typeof(DnsEndPoint);

		/// <inheritdoc />
		public JsonContract CreateContract () {
			var hostProp =
				JsonContractKit<DnsEndPoint>.ReadOnlyProperty(
					nameof(DnsEndPoint.Host),
					dnsEndPoint => dnsEndPoint.Host);
			var portProp =
				JsonContractKit<DnsEndPoint>.ReadOnlyProperty(
					nameof(DnsEndPoint.Port), dnsEndPoint => dnsEndPoint.Port);
			var addressFamilyProp =
				JsonContractKit<DnsEndPoint>.ReadOnlyProperty(
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
					JsonContractKit<DnsEndPoint>.OverrideCreator(
						(String host, Int32 port, AddressFamily addressFamily) =>
							new DnsEndPoint(host, port, addressFamily))
			};
		}
	}
}
