// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Abstractions {
	using Newtonsoft.Json;
	using System;

	public sealed class JsonSerializerAdapter: IJsonSerializer {
		JsonSerializer serializer { get; }

		public JsonSerializerAdapter (JsonSerializer serializer) => this.serializer = serializer;

		/// <inheritdoc />
		public void Write (JsonWriter jsonWriter, Object? maybeData) =>
			serializer.Serialize(jsonWriter, maybeData);

		/// <inheritdoc />
		public Object? MayRead (JsonReader jsonReader, Type dataType) =>
			serializer.Deserialize(jsonReader, dataType);
	}
}
