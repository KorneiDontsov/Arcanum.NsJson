// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;

	public sealed class JitCollectionJsonConverter<T>: JitSequenceToJsonConverter<T>, IJsonConverter {
		static Boolean canHaveNullItems { get; } =
			! typeof(T).IsValueType
			|| typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);

		Boolean allowNullItems { get; }
		CreateCollectionFunc<T> createCollection { get; }
		FinalizeCollectionFunc<T> finalizeCollection { get; }

		public JitCollectionJsonConverter
			(Boolean allowNullItems,
			 CreateCollectionFunc<T> createCollection,
			 FinalizeCollectionFunc<T> finalizeCollection) {
			this.allowNullItems = allowNullItems;
			this.createCollection = createCollection;
			this.finalizeCollection = finalizeCollection;
		}

		public JitCollectionJsonConverter
			(Boolean allowNullItems, CreateCollectionFunc<T> createCollection):
			this(allowNullItems, createCollection, finalizeCollection: it => it) { }

		/// <inheritdoc />
		public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
			reader.CurrentTokenMustBe(JsonToken.StartArray);
			reader.ReadNext();

			var collection = createCollection(locals);

			if(collection.IsReadOnly) {
				var msg = $"{collection.GetType()} is readonly collection and cannot be deserialized.";
				throw new JsonSerializationException(msg);
			}

			var acceptNullItems = canHaveNullItems && allowNullItems;

			static Exception NullItemError (JsonReader reader, UInt64 itemNumber) =>
				reader.Exception("Item {0} is null which is not allowed for the current collection.", itemNumber);

			var itemNumber = 0ul;
			while(reader.TokenType != JsonToken.EndArray) {
				var item =
					(reader.TokenType, acceptNullItems) switch {
						(JsonToken.Null, true) => default!, // always null
						(JsonToken.Null, false) => throw NullItemError(reader, itemNumber),
						_ => serializer.Read<T>(reader)
					};
				collection.Add(item);
				reader.ReadNext();
				itemNumber += 1;
			}

			return finalizeCollection(collection);
		}
	}
}
