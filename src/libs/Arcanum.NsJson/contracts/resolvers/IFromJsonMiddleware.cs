// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	public interface IFromJsonMiddleware {
		Object? Read (JsonReader reader, JsonSerializer serializer, ReadJson next);
	}
}
