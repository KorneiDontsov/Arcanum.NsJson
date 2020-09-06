// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	public interface IJsonSerializer {
		JsonSerializerSetup setup { get; }

		/// <exception cref = "JsonException" />
		void Write (JsonWriter jsonWriter, Object? maybeData, Action<ILocalsCollection>? configureLocals = null);

		/// <exception cref = "JsonException" />
		Object? MayRead (JsonReader jsonReader, Type dataType, Action<ILocalsCollection>? configureLocals = null);
	}
}
