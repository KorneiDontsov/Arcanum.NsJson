// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	public interface IJsonReadMiddleware {
		Object? ReadJson (JsonReader reader, Object? existingValue, JsonSerializer serializer, ReadJson next);
	}
}
