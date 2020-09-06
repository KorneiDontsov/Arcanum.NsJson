// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;

class MemoizeFunc {
	delegate TResult InvocationInvocation<TInvocation, TArg, TResult>
		(TInvocation invocation, TArg arg, Func<TArg, TResult> self);

	class Implementation<TInvocation, TArg, TResult> {
		ConcurrentDictionary<TArg, TResult> dict { get; } = new ConcurrentDictionary<TArg, TResult>();
		Func<TArg, TResult> invoke { get; }
		TResult GetOrInvoke (TArg arg) => dict.GetOrAdd(arg, invoke);

		public Func<TArg, TResult> self { get; }
		TInvocation invocation { get; }
		InvocationInvocation<TInvocation, TArg, TResult> invocationInvocation { get; }
		TResult Invoke (TArg arg) => invocationInvocation(invocation, arg, self);

		public Implementation
			(TInvocation invocation, InvocationInvocation<TInvocation, TArg, TResult> invocationInvocation) {
			invoke = Invoke;
			self = GetOrInvoke;

			this.invocation = invocation;
			this.invocationInvocation = invocationInvocation;
		}
	}

	public delegate TResult Invocation<TArg, TResult> (TArg arg, Func<TArg, TResult> self);

	public static Func<TArg, TResult> Create<TArg, TResult> (Invocation<TArg, TResult> invocation) =>
		new Implementation<Invocation<TArg, TResult>, TArg, TResult>(
				invocation,
				(invocation, arg, self) => invocation(arg, self))
			.self;

	public static Func<TArg, TResult> Create<TArg, TResult> (Func<TArg, TResult> invocation) =>
		new Implementation<Func<TArg, TResult>, TArg, TResult>(
				invocation,
				(invocation, arg, self) => invocation(arg))
			.self;
}
