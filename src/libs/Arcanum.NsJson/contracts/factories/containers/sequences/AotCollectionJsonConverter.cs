// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public sealed class AotCollectionJsonConverter: AotSequenceToJsonConverter, IJsonConverter {
		class CollectionMethods {
			public MethodInfo isReadOnly { get; }
			public MethodInfo add { get; }

			public CollectionMethods (MethodInfo isReadOnly, MethodInfo add) {
				this.isReadOnly = isReadOnly;
				this.add = add;
			}
		}

		static Func<Type, CollectionMethods> getCollectionMethods { get; } =
			MemoizeFunc.Create<Type, CollectionMethods>(
				(itemType, self) => {
					var t = typeof(ICollection<>).MakeGenericType(itemType);
					// ReSharper disable once PossibleNullReferenceException
					var isReadOnly = t.GetProperty(nameof(ICollection<Object>.IsReadOnly))!.GetGetMethod();
					var add = t.GetMethod(nameof(ICollection<Object>.Add))!;
					return new CollectionMethods(isReadOnly, add);
				});

		Type itemType { get; }
		Boolean allowNullItems { get; }
		Func<ILocalsCollection, Object> createCollection { get; }
		Func<Object, Object> finalizeCollection { get; }
		Boolean canHaveNullItems { get; }

		public AotCollectionJsonConverter
			(Type itemType,
			 Boolean allowNullItems,
			 Func<ILocalsCollection, Object> createCollection,
			 Func<Object, Object> finalizeCollection) {
			this.itemType = itemType;
			this.allowNullItems = allowNullItems;
			this.createCollection = createCollection;
			this.finalizeCollection = finalizeCollection;
			canHaveNullItems =
				! itemType.IsValueType
				|| itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public AotCollectionJsonConverter
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
					$"{nameof(AotCollectionJsonConverter)} configuration error: "
					+ $"'{nameof(createCollection)}' returned object that is not collection.";
				throw new JsonSerializationException(msg);
			}
			else if(! collectionItemType.IsAssignableFrom(itemType)) {
				var msg =
					$"{nameof(AotCollectionJsonConverter)} configuration error: "
					+ $"{nameof(createCollection)} returned collection "
					+ $"which item type is not assignable from {collectionItemType}";
				throw new JsonSerializationException(msg);
			}

			var methods = getCollectionMethods(collectionItemType);

			var isReadOnly = (Boolean) methods.isReadOnly.Invoke(collection, Array.Empty<Object>());
			if(isReadOnly) {
				var msg = $"{collection.GetType()} is readonly collection and cannot be deserialized.";
				throw new JsonSerializationException(msg);
			}

			var itemAsArray = new Object?[1];
			var acceptNullItems = canHaveNullItems && allowNullItems;

			static Exception NullItemError (JsonReader reader, UInt64 itemNumber) =>
				reader.Exception("Item {0} is null which is not allowed for the current collection.", itemNumber);

			var itemNumber = 0ul;
			while(reader.TokenType != JsonToken.EndArray) {
				itemAsArray[0] =
					(reader.TokenType, acceptNullItems) switch {
						(JsonToken.Null, true) => null,
						(JsonToken.Null, false) => throw NullItemError(reader, itemNumber),
						_ => serializer.Read(reader, itemType)
					};
				methods.add.Invoke(collection, itemAsArray);
				reader.ReadNext();
				itemNumber += 1;
			}

			return finalizeCollection(collection);
		}
	}
}
