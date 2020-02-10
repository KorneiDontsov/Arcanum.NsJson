// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class ValueProvider<TTarget, TValue>: IValueProvider {
		Func<TTarget, TValue> get { get; }
		Action<TTarget, TValue> set { get; }

		public ValueProvider (Func<TTarget, TValue> get, Action<TTarget, TValue> set) {
			this.get = get;
			this.set = set;
		}

		/// <inheritdoc />
		public void SetValue (Object target, Object? value) => set((TTarget) target, (TValue) value!);

		/// <inheritdoc />
		public Object? GetValue (Object target) => get((TTarget) target);
	}
}
