// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;

	public sealed class AotSetFromJsonConverter: IFromJsonConverter {
		Type itemType { get; }
		Func<ILocalsCollection, Object> createSet { get; }
		Func<Object, Object> finalizeSet { get; }
		Boolean acceptNullItems { get; }

		public AotSetFromJsonConverter
			(Type itemType,
			 Boolean allowNullItems,
			 Func<ILocalsCollection, Object> createSet,
			 Func<Object, Object> finalizeSet) {
			this.itemType = itemType;
			this.createSet = createSet;
			this.finalizeSet = finalizeSet;
			acceptNullItems =
				allowNullItems
				&& (! itemType.IsValueType
				    || itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public AotSetFromJsonConverter
			(Type itemType,
			 Boolean allowNullItems,
			 Func<ILocalsCollection, Object> createSet):
			this(itemType, allowNullItems, createSet, finalizeSet: set => set) { }

		/// <inheritdoc />
		public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
			reader.CurrentTokenMustBe(JsonToken.StartArray);
			reader.ReadNext();

			var samples = locals.MaybeSamples();

			var set = samples?.createSelfSample?.Invoke() ?? createSet(locals);
			var setType = set.GetType();
			var setItemType = setType.MaybeUnderlyingTypeIfHasGenericInterface(typeof(ICollection<>));
			if(setItemType is null || ! setItemType.IsAssignableFrom(itemType)) {
				var msg =
					$"Expected sample to be assignable to {nameof(ISet<Object>)}<{itemType}>, "
					+ $"but actual {setType}.";
				throw new JsonSerializationException(msg);
			}
			else if(AotCollectionMethods.forItemType(setItemType) is var collectionMethods
			        && collectionMethods.isReadOnly.Invoke(set, Array.Empty<Object>()) is true)
				throw new JsonSerializationException(
					$"{setType} is read-only collection and cannot be deserialized.");
			else {
				var setMethods = AotSetMethods.forItemType(setItemType);

				// Set samples for items.
				if(samples?.createItemSample is {} createItemSample)
					locals.SetSamples(new LocalSamples(dataType: itemType, createSelfSample: createItemSample));

				var itemAsArray = new Object?[1];

				for(var itemNumber = 0ul; ! (reader.TokenType is JsonToken.EndArray); itemNumber += 1) {
					itemAsArray[0] =
						(reader.TokenType, acceptNullItems) switch {
							(JsonToken.Null, true) => null,
							(JsonToken.Null, false) =>
								throw reader.Exception(
									"Item {0} is null which is not allowed for the current collection.",
									itemNumber),
							_ => serializer.Read(reader, itemType)
						};
					if(setMethods.add.Invoke(set, itemAsArray) is false) {
						collectionMethods.remove.Invoke(set, itemAsArray);
						collectionMethods.add.Invoke(set, itemAsArray);
					}
					reader.ReadNext();
				}

				return finalizeSet(set);
			}
		}
	}
}
