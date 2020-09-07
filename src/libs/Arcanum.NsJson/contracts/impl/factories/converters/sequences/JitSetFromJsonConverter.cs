// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;

	public sealed class JitSetFromJsonConverter<T>: IFromJsonConverter {
		static Boolean canHaveNullItems { get; } =
			! typeof(T).IsValueType
			|| typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);

		Boolean allowNullItems { get; }
		CreateSetFunc<T> createSet { get; }
		FinalizeSetFunc<T> finalizeSet { get; }

		public JitSetFromJsonConverter
			(Boolean allowNullItems,
			 CreateSetFunc<T> createSet,
			 FinalizeSetFunc<T> finalizeSet) {
			this.allowNullItems = allowNullItems;
			this.createSet = createSet;
			this.finalizeSet = finalizeSet;
		}

		public JitSetFromJsonConverter (Boolean allowNullItems, CreateSetFunc<T> createSet):
			this(allowNullItems, createSet, finalizeSet: it => it) { }

		/// <inheritdoc />
		public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
			reader.CurrentTokenMustBe(JsonToken.StartArray);
			reader.ReadNext();

			var samples = locals.MaybeSamples();

			ISet<T> set;
			if(! (samples?.createSelfSample is {} createSelfSample))
				set = createSet(locals);
			else if(createSelfSample() is var sample && sample is ISet<T> setSample)
				set = setSample;
			else {
				var msg =
					$"Expected sample to be assignable to {nameof(ISet<T>)}<{typeof(T)}>, "
					+ $"but actual {sample.GetType()}.";
				throw new JsonSerializationException(msg);
			}

			if(set.IsReadOnly)
				throw new JsonSerializationException(
					$"{set.GetType()} is read-only set and cannot be deserialized.");
			else {
				// Set samples for items.
				if(samples?.createItemSample is {} createItemSample)
					locals.SetSamples(new LocalSamples(dataType: typeof(T), createSelfSample: createItemSample));

				var acceptNullItems = canHaveNullItems && allowNullItems;

				for(var itemNumber = 0ul; ! (reader.TokenType is JsonToken.EndArray); itemNumber += 1) {
					var item =
						(reader.TokenType, acceptNullItems) switch {
							(JsonToken.Null, true) => default!, // always null
							(JsonToken.Null, false) =>
								throw reader.Exception(
									"Item {0} is null which is not allowed for the current set.",
									itemNumber),
							_ => serializer.Read<T>(reader)
						};
					if(! set.Add(item)) {
						set.Remove(item);
						set.Add(item);
					}
					reader.ReadNext();
				}

				return finalizeSet(set);
			}
		}
	}
}
