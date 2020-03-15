// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Net;
	using System.Net.Sockets;

	public sealed class DnsEndPointJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if(request.dataType == typeof(DnsEndPoint))
				request.Return(
					new JsonObjectContractBuilder<DnsEndPoint>()
						.AddCreatorArg(nameof(DnsEndPoint.Host), d => d.Host)
						.AddCreatorArg(nameof(DnsEndPoint.Port), d => d.Port)
						.AddCreatorArg(
							nameof(DnsEndPoint.AddressFamily),
							d => d.AddressFamily,
							Required.Default,
							DefaultValueHandling.IgnoreAndPopulate,
							defaultValue: AddressFamily.Unspecified)
						.AddCreator(
							(String host, Int32 port, AddressFamily addressFamily) =>
								new DnsEndPoint(host, port, addressFamily))
						.Build());
		}
	}
}
