// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Runtime.CompilerServices;

class DefaultReferenceComparer: IEqualityComparer {
	/// <inheritdoc />
	public new Boolean Equals (Object x, Object y) => ReferenceEquals(x, y);

	/// <inheritdoc />
	public Int32 GetHashCode (Object obj) => RuntimeHelpers.GetHashCode(obj);
}
