// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;

	public interface IJsonMiddlewareRequest {
		Type dataType { get; }

		void Yield (IToJsonMiddleware toJsonMiddleware);

		void Yield (IFromJsonMiddleware fromJsonMiddleware);
	}
}
