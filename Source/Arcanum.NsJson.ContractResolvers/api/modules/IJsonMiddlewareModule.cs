// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public interface IJsonMiddlewareModule {
		/// <exception cref = "JsonContractException" />
		JsonConverter? MayCreateConverter (JsonContract contract);
	}
}
