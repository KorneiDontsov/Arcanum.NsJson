﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System.Collections.Generic;

	public delegate ISet<T> CreateSetFunc<T> (ILocalsCollection locals);
}
