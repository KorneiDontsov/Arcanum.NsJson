// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using System.Reflection;
	using static ReflectionFunctions;
	using static RuntimeModule;

	public sealed class SetInterfacesJsonArrayConverterFactory: IJsonArrayConverterFactory {
		static ImmutableHashSet<Type> handledTypeDefinitions { get; } =
			ImmutableHashSet.Create(typeof(ISet<>));

		// ReSharper disable once UnusedParameter.Local
		static ISet<T> CreateHashSet<T> (ILocalsCollection locals) =>
			new HashSet<T>();

		// ReSharper disable once UnusedParameter.Local
		static ISet<T> CreateSortedSet<T> (ILocalsCollection locals) =>
			new SortedSet<T>();

		static IFromJsonConverter CreateFromJsonConverter
			(Type collectionType, Type itemType, JsonArrayAttribute? jsonArrayAttribute) {
			var allowNullItems = jsonArrayAttribute?.AllowNullItems ?? true;
			var useSortedSet =
				itemType.GetUnderlyingTypesOfClosedGenericInterfaces(typeof(IEquatable<>))
					.All(t => ! t.IsAssignableFrom(itemType))
				&& itemType.GetUnderlyingTypesOfClosedGenericInterfaces(typeof(IComparable<>))
					.Any(t => t.IsAssignableFrom(itemType));
			if(isJit) {
				var createSetMethodName = useSortedSet ? nameof(CreateSortedSet) : nameof(CreateHashSet);
				var createSet =
					MethodBase.GetCurrentMethod().DeclaringType
						.GetGenericMethodDefinition(createSetMethodName, BindingFlags.Static | BindingFlags.NonPublic)
						.MakeGenericMethod(itemType)
						.CreateGenericDelegate(typeof(CreateSetFunc<>), itemType);
				return typeof(JitSetFromJsonConverter<>).MakeGenericType(itemType)
					.ConstructAs<IFromJsonConverter>(allowNullItems, createSet);
			}
			else {
				if(MayMakeGenericType(useSortedSet ? typeof(SortedSet<>) : typeof(HashSet<>), itemType) is {} setType)
					return new AotSetFromJsonConverter(
						itemType,
						allowNullItems,
						createSet: locals => setType.Construct());
				else
					return new AotSetFromJsonConverter(
						itemType,
						allowNullItems,
						createSet: locals =>
							throw new JsonSerializationException(
								$"Cannot create {collectionType} because of AOT. Add sample."));
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(request.dataType.IsGenericType
			   && handledTypeDefinitions.Contains(request.dataType.GetGenericTypeDefinition())) {
				var itemType = request.dataType.GetGenericArguments()[0];
				var toJsonConverter = AnySequenceJsonArrayConverterFactory.CreateToJsonConverter(itemType);
				var fromJsonConverter = CreateFromJsonConverter(request.dataType, itemType, jsonArrayAttribute);
				request.Return(toJsonConverter, fromJsonConverter);
			}
		}
	}
}
