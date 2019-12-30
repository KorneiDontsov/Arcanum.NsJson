// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;

	static class JsonContractKit<T> {
		class ReadOnlyPropertyValueProvider<TProperty>: IValueProvider {
			Func<T, TProperty> get { get; }
			public ReadOnlyPropertyValueProvider (Func<T, TProperty> get) => this.get = get;

			/// <inheritdoc />
			public void SetValue (Object target, Object? value) => throw new NotSupportedException();

			/// <inheritdoc />
			public Object? GetValue (Object target) => get((T) target);
		}

		public static JsonProperty ReadOnlyProperty<TProperty>
		(String propertyName,
		 Func<T, TProperty> getValue,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) =>
			new JsonProperty {
				PropertyName = propertyName,
				UnderlyingName = propertyName,
				PropertyType = typeof(TProperty),
				DeclaringType = typeof(T),
				Readable = true,
				Writable = false,
				ValueProvider = new ReadOnlyPropertyValueProvider<TProperty>(getValue),
				HasMemberAttribute = true,
				Required = required,
				DefaultValueHandling = defaultValueHandling,
				DefaultValue = defaultValue
			};

		#nullable disable warnings
		public static ObjectConstructor<Object> OverrideCreator<T1, T2, T3> (Func<T1, T2, T3, T> creator) =>
			args => creator((T1) args![0], (T2) args![1], (T3) args![2]);
		#nullable restore warnings
	}
}
