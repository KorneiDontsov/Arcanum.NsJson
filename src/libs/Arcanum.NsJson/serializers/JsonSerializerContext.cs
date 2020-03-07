// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;

	class JsonSerializerContext: IDisposable {
		class LocalsCollection: ILocalsCollection {
			public LocalsCollection? prototype { get; set; }

			Dictionary<Object, Object> items { get; } = new Dictionary<Object, Object>();

			/// <inheritdoc />
			public Object? this [Object key] {
				get => items.TryGetValue(key, out var value) ? value : prototype?[key];
				set {
					if (value is {})
						items[key] = value;
					else
						items.Remove(key);
				}
			}

			public void Clear () => items.Clear();
		}

		static ConcurrentBag<LocalsCollection> localsPool { get; } =
			new ConcurrentBag<LocalsCollection>();

		public Int32 threadId { get; set; }
		public Action<Int32> closeContext { get; set; } = null!;

		LocalsCollection? currentLocals { get; set; }

		public ILocalsCollection locals => currentLocals!;

		public void Capture () {
			var newLocals = localsPool.TryPeek(out var pooledLocals) ? pooledLocals : new LocalsCollection();
			newLocals.prototype = currentLocals;
			currentLocals = newLocals;
		}

		/// <inheritdoc />
		public void Dispose () {
			var freeLocals = currentLocals!;
			currentLocals = freeLocals.prototype;

			freeLocals.Clear();
			localsPool.Add(freeLocals);

			if (currentLocals is null)
				closeContext(threadId);
		}
	}
}
