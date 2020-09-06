// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;

	public static class LocalsCollectionFunctions {
		public readonly struct LocalUsageScope: IDisposable {
			readonly ILocalsCollection locals;
			readonly Object key;
			readonly Object? restoreValue;

			internal LocalUsageScope (ILocalsCollection locals, Object key, Object? restoreValue) {
				this.locals = locals;
				this.key = key;
				this.restoreValue = restoreValue;
			}

			/// <inheritdoc />
			public void Dispose () =>
				locals[key] = restoreValue;
		}

		public static LocalUsageScope With (this ILocalsCollection locals, Object key, Object value) {
			var restoreValue = locals[key];
			locals[key] = value;
			return new LocalUsageScope(locals, key, restoreValue);
		}
	}
}
