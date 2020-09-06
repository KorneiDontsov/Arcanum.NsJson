// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using Newtonsoft.Json.Serialization;
using System;

class ReadOnlyValueProvider<TTarget, TValue>: IValueProvider {
	Func<TTarget, TValue> get { get; }
	public ReadOnlyValueProvider (Func<TTarget, TValue> get) => this.get = get;

	/// <inheritdoc />
	public void SetValue (Object target, Object? value) => throw new NotSupportedException();

	/// <inheritdoc />
	public Object? GetValue (Object target) => get((TTarget) target);
}
