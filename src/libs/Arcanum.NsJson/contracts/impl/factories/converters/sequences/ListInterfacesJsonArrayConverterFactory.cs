// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Reflection;
	using static ReflectionFunctions;
	using static RuntimeModule;

	public sealed class ListInterfacesJsonArrayConverterFactory: IJsonArrayConverterFactory {
		static ImmutableHashSet<Type> handledTypeDefinitions { get; } =
			ImmutableHashSet.CreateRange(
				new[] {
					typeof(IEnumerable<>),
					typeof(IReadOnlyCollection<>),
					typeof(IReadOnlyList<>),
					typeof(ICollection<>),
					typeof(IList<>)
				});

		// ReSharper disable once UnusedParameter.Local
		static ICollection<T> CreateCollection<T> (ILocalsCollection locals) =>
			new List<T>();

		static IFromJsonConverter CreateFromJsonConverter
			(Type collectionType, Type itemType, JsonArrayAttribute? jsonArrayAttribute) {
			var allowNullItems = jsonArrayAttribute?.AllowNullItems ?? true;
			if(isJit) {
				var createCollection =
					MethodBase.GetCurrentMethod()
						.DeclaringType.GetGenericMethodDefinition(
							nameof(CreateCollection),
							BindingFlags.Static | BindingFlags.NonPublic)
						.MakeGenericMethod(itemType)
						.CreateGenericDelegate(typeof(CreateCollectionFunc<>), itemType);
				return typeof(JitCollectionFromJsonConverter<>).MakeGenericType(itemType)
					.ConstructAs<IFromJsonConverter>(allowNullItems, createCollection);
			}
			else if(MayMakeGenericType(typeof(List<>), itemType) is {} listType)
				return new AotCollectionFromJsonConverter(
					itemType,
					allowNullItems,
					createCollection: locals => listType.Construct());
			else
				return new AotCollectionFromJsonConverter(
					itemType,
					allowNullItems,
					createCollection: locals =>
						throw new JsonSerializationException(
							$"Cannot create {collectionType} because of AOT. Add sample."));
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
