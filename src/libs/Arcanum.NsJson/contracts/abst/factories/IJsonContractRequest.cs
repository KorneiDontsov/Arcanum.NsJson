// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public interface IJsonContractRequest {
		Type dataType { get; }

		void Return (JsonContract contract);
	}
}
