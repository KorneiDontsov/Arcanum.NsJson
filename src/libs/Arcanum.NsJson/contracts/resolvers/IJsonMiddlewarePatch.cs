// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;

	public interface IJsonMiddlewarePatch {
		/// <exception cref = "JsonContractException" />
		void Configure (JsonContract contract, IJsonMiddlewareBuilder middlewareBuilder);
	}
}
