// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Reflection;

	static class ContainerConverterFactoryModule {
		static Func<Type, JsonContainerAttribute?> maybeJsonContainerAttribute { get; } =
			MemoizeFunc.Create<Type, JsonContainerAttribute?>(
				(type, self) => {
					static Exception MultiplyAttributesError (Type type) =>
						new Exception($"Type {type} has multiply json container attributes.");

					using var e = type.GetCustomAttributes<JsonContainerAttribute>(inherit: false).GetEnumerator();
					if(e.MoveNext()) {
						var jsonContainerAttribute = e.Current;
						return ! e.MoveNext() ? jsonContainerAttribute : throw MultiplyAttributesError(type);
					}
					else if(type.BaseType is {} baseType)
						return self(baseType);
					else
						return null;
				});

		class JsonArrayConverterFactoryAdapter: IJsonConverterFactory {
			IJsonArrayConverterFactory factory { get; }

			public JsonArrayConverterFactoryAdapter (IJsonArrayConverterFactory factory) =>
				this.factory = factory;

			/// <inheritdoc />
			public void Handle (IJsonConverterRequest request) {
				var jsonContainerAttribute = maybeJsonContainerAttribute(request.dataType);
				var jsonArrayAttribute = jsonContainerAttribute as JsonArrayAttribute;
				if(jsonContainerAttribute is null || jsonArrayAttribute is {})
					factory.Handle(request, jsonArrayAttribute);
			}
		}

		public static IMicroContractResolverBuilder AddJsonArrayConverterFactory
			(this IMicroContractResolverBuilder builder, IJsonArrayConverterFactory converterFactory) =>
			builder.AddConverterFactory(new JsonArrayConverterFactoryAdapter(converterFactory));
	}
}
