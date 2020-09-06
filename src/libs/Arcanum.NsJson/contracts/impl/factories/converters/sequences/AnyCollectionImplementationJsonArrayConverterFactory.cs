// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using static ReflectionFunctions;
	using static RuntimeModule;

	public sealed class AnyCollectionImplementationJsonArrayConverterFactory: IJsonArrayConverterFactory {
		static Exception NoDefaultConstructorError (Type dataType) {
			var msg = $"Failed to create {dataType} because it has no default constructor. Add sample.";
			throw new JsonSerializationException(msg);
		}

		// ReSharper disable once UnusedParameter.Local
		static ICollection<T> CreateCollectionWithoutDefaultConstructor<T, TCollection> (ILocalsCollection locals)
			where TCollection: ICollection<T> =>
			throw NoDefaultConstructorError(typeof(TCollection));

		// ReSharper disable once UnusedParameter.Local
		static ICollection<T> CreateCollection<T, TCollection> (ILocalsCollection locals)
			where TCollection: ICollection<T>, new() =>
			new TCollection();

		static IFromJsonConverter CreateFromJsonConverter
			(Type collectionType, Type itemType, JsonArrayAttribute? jsonArrayAttribute) {
			var allowNullItems = jsonArrayAttribute?.AllowNullItems ?? true;
			var defaultConstructor = collectionType.GetConstructor(Array.Empty<Type>());

			if(isJit) {
				var createCollectionMethodName =
					defaultConstructor is {}
						? nameof(CreateCollection)
						: nameof(CreateCollectionWithoutDefaultConstructor);
				var createCollection =
					MethodBase.GetCurrentMethod().DeclaringType!
						.GetGenericMethodDefinition(
							createCollectionMethodName,
							BindingFlags.Static | BindingFlags.NonPublic)
						.MakeGenericMethod(itemType, collectionType)
						.CreateGenericDelegate(typeof(CreateCollectionFunc<>), itemType);
				return typeof(JitCollectionFromJsonConverter<>).MakeGenericType(itemType)
					.ConstructAs<IFromJsonConverter>(allowNullItems, createCollection);
			}
			else if(defaultConstructor is {})
				return new AotCollectionFromJsonConverter(
					itemType,
					allowNullItems,
					createCollection: locals => defaultConstructor.Invoke(Array.Empty<Object>()));
			else
				return new AotCollectionFromJsonConverter(
					itemType,
					allowNullItems,
					createCollection: locals => throw NoDefaultConstructorError(collectionType));
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(! request.dataType.IsAbstract
			   && request.dataType.HasOpenGenericInterface(typeof(ICollection<>), out var itemType)) {
				var toJsonConverter = AnySequenceJsonArrayConverterFactory.CreateToJsonConverter(itemType);
				var fromJsonConverter = CreateFromJsonConverter(request.dataType, itemType, jsonArrayAttribute);
				request.Return(toJsonConverter, fromJsonConverter);
			}
		}
	}
}
