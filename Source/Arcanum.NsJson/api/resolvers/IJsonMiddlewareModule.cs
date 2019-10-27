// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	interface IJsonMiddlewareModule {
		JsonConverter? MayCreateConverter (JsonContract contract);
	}
}
