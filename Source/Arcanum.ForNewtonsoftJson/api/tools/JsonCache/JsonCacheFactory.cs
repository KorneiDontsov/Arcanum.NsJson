// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.ForNewtonsoftJson {
	using Microsoft.IO;

	public sealed class JsonCacheFactory {
		RecyclableMemoryStreamManager implStreamManager { get; }

		public static JsonCacheFactory shared { get; } = new JsonCacheFactory();

		public JsonCacheFactory () =>
			implStreamManager = new RecyclableMemoryStreamManager();

		public JsonCache GetCache () => new JsonCache(implStreamManager.GetStream());
	}
}
