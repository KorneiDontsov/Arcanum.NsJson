// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	public interface IFromJsonConverter {
		Object? Read (JsonReader reader, JsonSerializer serializer);
	}
}
