// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using static RuntimeModule;

	public sealed class ArrayJsonArrayConverterFactory: IJsonArrayConverterFactory {
		// ReSharper disable once UnusedParameter.Local
		static ICollection<T> CreateCollection<T> (ILocalsCollection locals) =>
			new List<T>();

		// ReSharper disable once UnusedParameter.Local
		static Object FinalizeCollection<T> (ICollection<T> collection) {
			var count = collection.Count;
			if(count is 0)
				return Array.Empty<T>();
			else {
				var array = new T[count];
				collection.CopyTo(array, 0);
				return array;
			}
		}

		static IFromJsonConverter CreateFromJsonConverter
			(Type arrayType, Type itemType, JsonArrayAttribute? jsonArrayAttribute) {
			var allowNullItems = jsonArrayAttribute?.AllowNullItems ?? true;

			if(isJit) {
				var currentType = MethodBase.GetCurrentMethod().DeclaringType!;
				var createCollection =
					currentType.GetGenericMethodDefinition(
							nameof(CreateCollection),
							BindingFlags.Static | BindingFlags.NonPublic)
						.MakeGenericMethod(itemType)
						.CreateGenericDelegate(typeof(CreateCollectionFunc<>), itemType);
				var finalizeCollection =
					currentType.GetGenericMethodDefinition(
							nameof(FinalizeCollection),
							BindingFlags.Static | BindingFlags.NonPublic)
						.MakeGenericMethod(itemType)
						.CreateGenericDelegate(typeof(FinalizeCollectionFunc<>), itemType);
				return typeof(JitCollectionFromJsonConverter<>).MakeGenericType(itemType)
					.ConstructAs<IFromJsonConverter>(allowNullItems, createCollection, finalizeCollection);
			}
			else
				return new AotCollectionFromJsonConverter(
					itemType,
					allowNullItems,
					createCollection: locals => new List<Object>(),
					finalizeCollection: collection => {
						var methods = AotCollectionMethods.forItemType(typeof(Object));
						var count = (Int32) methods.count.Invoke(collection, Array.Empty<Object>());
						var array = Array.CreateInstance(itemType, count);
						if(count > 0) methods.copyTo.Invoke(collection, new Object[] { array, 0 });
						return array;
					});
		}

		static Type GetArrayItemType (Type arrayType) {
			var type = arrayType.MaybeUnderlyingTypeIfHasGenericInterface(typeof(ICollection<>));
			if(type is null) {
				var msg =
					$"Fatal error: {arrayType} is array type but don't implement interface "
					+ $"{typeof(ICollection<>)}.";
				throw new JsonContractException(msg);
			}
			else
				return type;
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(request.dataType.IsArray && request.dataType.GetArrayRank() is 1) {
				var itemType = GetArrayItemType(request.dataType);
				var toJsonConverter = AnySequenceJsonArrayConverterFactory.CreateToJsonConverter(itemType);
				var fromJsonConverter = CreateFromJsonConverter(request.dataType, itemType, jsonArrayAttribute);
				request.Return(toJsonConverter, fromJsonConverter);
			}
		}
	}
}
