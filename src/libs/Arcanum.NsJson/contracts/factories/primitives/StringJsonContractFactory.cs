// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class StringJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if(request.dataType == typeof(String))
				request.Return(new JsonStringContract(typeof(String)));
		}
	}
}
