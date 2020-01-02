// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Net;

	public sealed class IpEndPointJsonContractCreator: IJsonContractCreator {
		/// <inheritdoc />
		public JsonContract CreateContract () =>
			new JsonObjectContractBuilder<IPEndPoint>()
				.AddCreatorArg(nameof(IPEndPoint.Address), d => d.Address)
				.AddCreatorArg(nameof(IPEndPoint.Port), d => d.Port)
				.AddCreator((IPAddress address, Int32 port) => new IPEndPoint(address, port))
				.Build();
	}
}
