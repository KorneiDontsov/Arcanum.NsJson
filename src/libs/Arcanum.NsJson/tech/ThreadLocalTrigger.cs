// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Threading;

readonly struct ThreadLocalTrigger: IDisposable {
	readonly ThreadLocal<Boolean> threadLocal;
	readonly Boolean previousState;

	public ThreadLocalTrigger (ThreadLocal<Boolean> threadLocal) {
		this.threadLocal = threadLocal;
		previousState = threadLocal.Value;
		threadLocal.Value = true;
	}

	/// <inheritdoc />
	public void Dispose () => threadLocal.Value = previousState;
}
