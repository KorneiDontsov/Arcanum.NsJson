﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json.Serialization;

	public interface IJsonContractModule {
		JsonContract CreateContract (JsonContract baseContract);
	}
}
