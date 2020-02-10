// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Net;
	using System.Net.Sockets;

	public sealed class DnsEndPointJsonContractCreator: IJsonContractCreator {
		/// <inheritdoc />
		public JsonContract CreateJsonContract () =>
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
				.Build();
	}
}
