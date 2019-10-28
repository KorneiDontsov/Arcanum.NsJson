// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Abstractions {
	using Newtonsoft.Json;
	using System;

	public interface IJsonSerializer {
		/// <exception cref = "JsonException" />
		void Write (JsonWriter jsonWriter, Object? maybeData);

		/// <exception cref = "JsonException" />
		Object? MayRead (JsonReader jsonReader, Type dataType);
	}
}
