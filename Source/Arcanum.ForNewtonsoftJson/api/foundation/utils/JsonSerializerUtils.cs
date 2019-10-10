// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.ForNewtonsoftJson {
	using Newtonsoft.Json;
	using System;

	public static class JsonSerializerUtils {
		public static Object? DeserializeWithoutMiddleware (
		this JsonSerializer serializer, JsonReader reader, Type objectType) {
			using (ArcanumJsonContractResolver.ResolveContractArgs.Set(withoutMiddleware: true))
				return serializer.Deserialize(reader, objectType);
		}

		public static void SerializeWithoutMiddleware (
		this JsonSerializer serializer, JsonWriter writer, Object? value, Type? objectType = null) {
			using (ArcanumJsonContractResolver.ResolveContractArgs.Set(withoutMiddleware: true))
				serializer.Serialize(writer, value, objectType);
		}
	}
}
