// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractModules {
	using Microsoft.IO;

	class JsonCacheFactory {
		RecyclableMemoryStreamManager implStreamManager { get; }

		public static JsonCacheFactory shared { get; } = new JsonCacheFactory();

		public JsonCacheFactory () =>
			implStreamManager = new RecyclableMemoryStreamManager();

		public JsonCache GetCache () => new JsonCache(implStreamManager.GetStream());
	}
}
