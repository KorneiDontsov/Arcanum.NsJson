// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.IO;

namespace Arcanum.ForNewtonsoftJson
{
	public sealed class JsonCacheFactory
	{
		private RecyclableMemoryStreamManager _implStreamManager { get; }

		public static JsonCacheFactory shared { get; } = new JsonCacheFactory();

		public JsonCacheFactory ()
		{
			_implStreamManager = new RecyclableMemoryStreamManager();
		}

		public JsonCache GetCache ()
		{
			return new JsonCache(_implStreamManager.GetStream());
		}
	}
}
