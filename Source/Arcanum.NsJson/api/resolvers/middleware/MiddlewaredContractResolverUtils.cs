// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	static class MiddlewaredContractResolverUtils {
		public static Object? DeserializeWithoutMiddleware (
		this JsonSerializer serializer, JsonReader reader, Type objectType) {
			using (MiddlewaredContractResolverArgs.Set(withoutMiddleware: true))
				return serializer.Deserialize(reader, objectType);
		}

		public static void SerializeWithoutMiddleware (
		this JsonSerializer serializer, JsonWriter writer, Object? value, Type? objectType = null) {
			using (MiddlewaredContractResolverArgs.Set(withoutMiddleware: true))
				serializer.Serialize(writer, value, objectType);
		}
	}
}
