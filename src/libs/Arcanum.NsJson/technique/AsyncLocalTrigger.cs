// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;
	using System.Threading;

	public readonly struct AsyncLocalTrigger: IDisposable {
		readonly AsyncLocal<Boolean> asyncLocal;
		readonly Boolean previousState;

		public AsyncLocalTrigger (AsyncLocal<Boolean> asyncLocal) {
			this.asyncLocal = asyncLocal;
			previousState = asyncLocal.Value;
			asyncLocal.Value = true;
		}

		/// <inheritdoc />
		public void Dispose () => asyncLocal.Value = previousState;
	}
}
