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
			ImmutableHashSet<Type>.Empty
				.Add(typeof(IEnumerable<>))
				.Add(typeof(IReadOnlyCollection<>))
				.Add(typeof(IReadOnlyList<>))
				.Add(typeof(ICollection<>))
				.Add(typeof(IList<>));

		// ReSharper disable once UnusedParameter.Local
		static ICollection<T> CreateCollection<T> (ILocalsCollection locals) =>
			new List<T>();

		static Type? MayMakeListType (Type itemType) {
			try {
				return typeof(List<>).MakeGenericType(itemType);
			}
			catch {
				return null;
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(request.dataType.IsGenericType
			   && handledTypeDefinitions.Contains(request.dataType.GetGenericTypeDefinition())) {
				var itemType = request.dataType.GetGenericArguments()[0];
				var allowNullItems = jsonArrayAttribute?.AllowNullItems ?? true;

				IJsonConverter converter;
				if(isJit) {
					var createCollection =
						MethodBase.GetCurrentMethod().DeclaringType!
							.GetGenericMethodDefinition(
								nameof(CreateCollection),
								BindingFlags.Static | BindingFlags.NonPublic)
							.MakeGenericMethod(itemType)
							.CreateGenericDelegate(typeof(CreateCollectionFunc<>), itemType);
					converter =
						typeof(JitCollectionJsonConverter<>).MakeGenericType(itemType)
							.ConstructAs<IJsonConverter>(allowNullItems, createCollection);
				}
				else if(MayMakeListType(itemType) is {} listType)
					converter =
						new AotCollectionJsonConverter(
							itemType,
							allowNullItems,
							createCollection: locals => listType.Construct());
				else {
					var dataType = request.dataType;
					converter =
						new AotCollectionJsonConverter(
							itemType,
							allowNullItems,
							createCollection: locals => {
								var msg = $"Cannot create {dataType} because of AOT. Add sample.";
								throw new JsonSerializationException(msg);
							});
				}

				request.Return(converter);
			}
		}
	}
}
