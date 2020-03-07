// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Net;

	public sealed class IpEndPointJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if (request.dataType == typeof(IPEndPoint))
				request.Return(
					new JsonObjectContractBuilder<IPEndPoint>()
						.AddCreatorArg(nameof(IPEndPoint.Address), d => d.Address)
						.AddCreatorArg(nameof(IPEndPoint.Port), d => d.Port)
						.AddCreator((IPAddress address, Int32 port) => new IPEndPoint(address, port))
						.Build());
		}
	}
}
