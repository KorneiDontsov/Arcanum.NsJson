// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;

class MemoizeFunc {
	public delegate TResult Invocation<TArg, TResult> (TArg arg, Func<TArg, TResult> self);

	class Implementation<TArg, TResult> {
		ConcurrentDictionary<TArg, TResult> dict { get; } = new ConcurrentDictionary<TArg, TResult>();
		Func<TArg, TResult> invoke { get; }
		TResult GetOrInvoke (TArg arg) => dict.GetOrAdd(arg, invoke);

		public Func<TArg, TResult> self { get; }
		Invocation<TArg, TResult> invocation { get; }
		TResult Invoke (TArg arg) => invocation(arg, self);

		public Implementation (Invocation<TArg, TResult> invocation) {
			this.invocation = invocation;
			invoke = Invoke;
			self = GetOrInvoke;
		}
	}

	public static Func<TArg, TResult> Create<TArg, TResult> (Invocation<TArg, TResult> invocation) =>
		new Implementation<TArg, TResult>(invocation).self;
}
