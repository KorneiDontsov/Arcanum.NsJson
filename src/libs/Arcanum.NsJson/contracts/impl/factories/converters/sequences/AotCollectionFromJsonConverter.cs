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

			var collection = createCollection(locals);
			var collectionItemType =
				collection.GetType().MaybeUnderlyingTypeIfHasGenericInterface(typeof(ICollection<>));
			if(collectionItemType is null) {
				var msg =
					$"{nameof(AotCollectionFromJsonConverter)} configuration error: "
					+ $"'{nameof(createCollection)}' returned object that is not collection.";
				throw new JsonSerializationException(msg);
			}
			else if(! collectionItemType.IsAssignableFrom(itemType)) {
				var msg =
					$"{nameof(AotCollectionFromJsonConverter)} configuration error: "
					+ $"{nameof(createCollection)} returned collection "
					+ $"which item type is not assignable from {collectionItemType}";
				throw new JsonSerializationException(msg);
			}
			else if(getCollectionMethods(collectionItemType) is var methods
			        && (Boolean) methods.isReadOnly.Invoke(collection, Array.Empty<Object>()))
				throw new JsonSerializationException(
					$"{collection.GetType()} is readonly collection and cannot be deserialized.");
			else {
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
