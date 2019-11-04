// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {

	public abstract class TestJsonSerializer {
		protected IJsonSerializer serializer { get; } = JsonFactory.defaultSerializer;
	}
}
