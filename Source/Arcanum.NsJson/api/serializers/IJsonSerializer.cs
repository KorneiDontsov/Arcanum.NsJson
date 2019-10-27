// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	public interface IJsonSerializer {
		void Write (JsonWriter jsonWriter, Object? maybeData);

		Object? MayRead (JsonReader jsonReader, Type dataType);
	}
}
