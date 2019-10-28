// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.Abstractions;
	using Newtonsoft.Json;

	public static class JsonFactory {
		public static IJsonSerializer defaultSerializer { get; } = Serializer(JsonSerializerConfig.@default);

		public static IJsonSerializer Serializer (JsonSerializerConfig serializerConfig) {
			var settings = JsonConfigUtils.CreateNsSerializerSettings(serializerConfig);
			var serializer = JsonSerializer.Create(settings);
			return new JsonSerializerAdapter(serializer);
		}
	}
}
