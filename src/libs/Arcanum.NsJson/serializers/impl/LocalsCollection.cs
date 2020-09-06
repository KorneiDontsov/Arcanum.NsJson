// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using static System.Threading.Interlocked;

	class LocalsCollection: ILocalsCollection {
		static Int64 freeToken;

		Dictionary<Object, Object?> items { get; } = new Dictionary<Object, Object?>();

		List<Int64> tokens { get; } = new List<Int64>(capacity: 2);

		JsonSerializationContext context = null!;

		LocalsCollection? prototype;

		/// <inheritdoc />
		public Object? this [Object key] {
			get => items.TryGetValue(key, out var value) ? value : prototype?[key];
			set {
				if(! ReferenceEquals(value, prototype?[key]))
					items[key] = value;
				else
					items.Remove(key);
			}
		}

		static ConcurrentBag<LocalsCollection> pool { get; } =
			new ConcurrentBag<LocalsCollection>();

		// items and tokens should be already empty.
		static void ReturnToPool (LocalsCollection locals) {
			locals.prototype = null;
			locals.context = null!;
			pool.Add(locals);
		}

		public static LocalsCollectionOwner Capture (JsonSerializationContext context) {
			if(! (context.locals is {items: {Count: 0}} locals)) {
				locals = pool.TryTake(out var pooledLocals) ? pooledLocals : new LocalsCollection();
				locals.context = context;
				locals.prototype = context.locals;
				context.locals = locals;
			}

			var token = Increment(ref freeToken);
			locals.tokens.Add(token);
			return new LocalsCollectionOwner(locals, token);
		}

		public void Free (Int64 token) {
			if(tokens.Count is 0)
				throw new InvalidOperationException("Not initialized.");
			else if(tokens.Count - 1 is var currentTokenIndex && tokens[currentTokenIndex] != token)
				throw new InvalidOperationException("Internal error: locals collection is already free.");
			else {
				items.Clear();
				tokens.RemoveAt(currentTokenIndex);

				if(tokens.Count is 0) {
					context.locals = prototype;
					ReturnToPool(this);
				}
			}
		}
	}
}
