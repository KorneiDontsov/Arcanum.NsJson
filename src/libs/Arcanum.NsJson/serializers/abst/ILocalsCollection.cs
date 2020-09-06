// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;

	public interface ILocalsCollection {
		Object? this [Object key] { get; set; }
	}
}
