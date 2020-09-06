// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;

	readonly struct LocalsCollectionOwner: IDisposable {
		public readonly LocalsCollection locals;

		readonly Int64 token;

		public LocalsCollectionOwner (LocalsCollection locals) {
			this.locals = locals;
			token = locals.token;
		}

		/// <inheritdoc />
		public void Dispose () =>
			locals.Free(token);
	}
}
