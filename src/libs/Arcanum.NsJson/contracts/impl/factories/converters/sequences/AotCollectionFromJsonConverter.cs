// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public sealed class AotCollectionFromJsonConverter: IFromJsonConverter {
		class CollectionMethods {
			public MethodInfo isReadOnly { get; }
			public MethodInfo add { get; }

			public CollectionMethods (MethodInfo isReadOnly, MethodInfo add) {
				this.isReadOnly = isReadOnly;
				this.add = add;
			}
		}

		static Func<Type, CollectionMethods> getCollectionMethods { get; } =
			MemoizeFunc.Create(
				(Type itemType) => {
					var t = typeof(ICollection<>).MakeGenericType(itemType);
					// ReSharper disable once PossibleNullReferenceException
					var isReadOnly = t.GetProperty(nameof(ICollection<Object>.IsReadOnly))!.GetGetMethod();
					var add = t.GetMethod(nameof(ICollection<Object>.Add))!;
					return new CollectionMethods(isReadOnly, add);
				});

		Type itemType { get; }
		Func<ILocalsCollection, Object> createCollection { get; }
		Func<Object, Object> finalizeCollection { get; }
		Boolean acceptNullItems { get; }

		public AotCollectionFromJsonConverter
			(Type itemType,
			 Boolean allowNullItems,
			 Func<ILocalsCollection, Object> createCollection,
			 Func<Object, Object> finalizeCollection) {
			this.itemType = itemType;
			this.createCollection = createCollection;
			this.finalizeCollection = finalizeCollection;
			acceptNullItems =
				allowNullItems
				&& (! itemType.IsValueType
				    || itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public AotCollectionFromJsonConverter
			(Type itemType,
			 Boolean allowNullItems,
			 Func<ILocalsCollection, Object> createCollection):
			this(itemType, allowNullItems, createCollection, finalizeCollection: collection => collection) { }

		/// <inheritdoc />
		public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
			reader.CurrentTokenMustBe(JsonToken.StartArray);
			reader.ReadNext();

			var samples = locals.MaybeSamples();

			var collection = samples?.createSelfSample?.Invoke() ?? createCollection(locals);
			var collectionType = collection.GetType();
			var collectionItemType = collectionType.MaybeUnderlyingTypeIfHasGenericInterface(typeof(ICollection<>));
			if(collectionItemType is null || ! collectionItemType.IsAssignableFrom(itemType)) {
				var msg =
					$"Expected sample to be assignable to {nameof(ICollection<Object>)}<{itemType}>, "
					+ $"but actual {collectionType}.";
				throw new JsonSerializationException(msg);
			}
			else if(getCollectionMethods(collectionItemType) is var methods
			        && (Boolean) methods.isReadOnly.Invoke(collection, Array.Empty<Object>()))
				throw new JsonSerializationException(
					$"{collectionType} is read-only collection and cannot be deserialized.");
			else {
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
					methods.add.Invoke(collection, itemAsArray);
					reader.ReadNext();
				}

				return finalizeCollection(collection);
			}
		}
	}
}
