// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;

	public sealed class JitCollectionFromJsonConverter<T>: IFromJsonConverter {
		static Boolean canHaveNullItems { get; } =
			! typeof(T).IsValueType
			|| typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);

		Boolean allowNullItems { get; }
		CreateCollectionFunc<T> createCollection { get; }
		FinalizeCollectionFunc<T> finalizeCollection { get; }

		public JitCollectionFromJsonConverter
			(Boolean allowNullItems,
			 CreateCollectionFunc<T> createCollection,
			 FinalizeCollectionFunc<T> finalizeCollection) {
			this.allowNullItems = allowNullItems;
			this.createCollection = createCollection;
			this.finalizeCollection = finalizeCollection;
		}

		public JitCollectionFromJsonConverter
			(Boolean allowNullItems, CreateCollectionFunc<T> createCollection):
			this(allowNullItems, createCollection, finalizeCollection: it => it) { }

		/// <inheritdoc />
		public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
			reader.CurrentTokenMustBe(JsonToken.StartArray);
			reader.ReadNext();

			var samples = locals.MaybeSamples();

			ICollection<T> collection;
			if(! (samples?.createSelfSample is {} createSelfSample))
				collection = createCollection(locals);
			else if(createSelfSample() is var sample && sample is ICollection<T> collectionSample)
				collection = collectionSample;
			else {
				var msg =
					$"Expected sample to be assignable to {nameof(ICollection<T>)}<{typeof(T)}>, "
					+ $"but actual {sample.GetType()}.";
				throw new JsonSerializationException(msg);
			}

			if(collection.IsReadOnly)
				throw new JsonSerializationException(
					$"{collection.GetType()} is read-only collection and cannot be deserialized.");
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
									"Item {0} is null which is not allowed for the current collection.",
									itemNumber),
							_ => serializer.Read<T>(reader)
						};
					collection.Add(item);
					reader.ReadNext();
				}

				return finalizeCollection(collection);
			}
		}
	}
}
