// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using static System.Threading.Interlocked;

	class LocalsCollection: ILocalsCollection {
		static ConcurrentBag<LocalsCollection> pool { get; } =
			new ConcurrentBag<LocalsCollection>();

		static Int64 freeToken;

		Dictionary<Object, Object> items { get; } = new Dictionary<Object, Object>();
		public Int64 token { get; private set; }
		JsonSerializationContext context = null!;
		LocalsCollection? prototype;

		/// <inheritdoc />
		public Object? this [Object key] {
			get => items.TryGetValue(key, out var value) ? value : prototype?[key];
			set {
				if(value is {})
					items[key] = value;
				else
					items.Remove(key);
			}
		}

		public static LocalsCollection Capture (JsonSerializationContext context) {
			var locals = pool.TryTake(out var pooledLocals) ? pooledLocals : new LocalsCollection();
			locals.token = Increment(ref freeToken);

			locals.context = context;
			(context.locals, locals.prototype) = (locals, context.locals);

			return locals;
		}

		public void Free (Int64 token) {
			if(this.token != token)
				throw new InvalidOperationException("Internal error: locals collection is already free.");
			else {
				(context.locals, prototype) = (prototype, null);
				context = null!;
				items.Clear();

				this.token = 0;
				pool.Add(this);
			}
		}
	}
}
