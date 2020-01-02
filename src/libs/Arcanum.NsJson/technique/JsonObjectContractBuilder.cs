// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Diagnostics.CodeAnalysis;

	class JsonObjectContractBuilder<T> {
		[return: NotNull]
		public delegate T Creator<T1, T2> ([AllowNull] T1 arg1, [AllowNull] T2 arg2);

		[return: NotNull]
		public delegate T Creator<T1, T2, T3> ([AllowNull] T1 arg1, [AllowNull] T2 arg2, [AllowNull] T3 arg3);

		JsonObjectContract contract { get; } =
			new JsonObjectContract(typeof(T)) {
				MemberSerialization = MemberSerialization.OptOut
			};

		static JsonProperty CreateProperty<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) =>
			new JsonProperty {
				PropertyName = propName,
				UnderlyingName = propName,
				PropertyType = typeof(TProp),
				DeclaringType = typeof(T),
				Readable = true,
				Writable = false,
				ValueProvider = new ReadOnlyValueProvider<T, TProp>(get),
				HasMemberAttribute = true,
				Required = required,
				DefaultValueHandling = defaultValueHandling,
				DefaultValue = defaultValue
			};

		static JsonProperty CreateProperty<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Action<T, TProp> set,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) =>
			new JsonProperty {
				PropertyName = propName,
				UnderlyingName = propName,
				PropertyType = typeof(TProp),
				DeclaringType = typeof(T),
				Readable = true,
				Writable = true,
				ValueProvider = new ValueProvider<T, TProp>(get, set),
				HasMemberAttribute = true,
				Required = required,
				DefaultValueHandling = defaultValueHandling,
				DefaultValue = defaultValue
			};

		public JsonObjectContractBuilder<T> AddProperty<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) {
			var prop = CreateProperty(propName, get, required, defaultValueHandling, defaultValue);
			contract.Properties.AddProperty(prop);
			return this;
		}

		public JsonObjectContractBuilder<T> AddProperty<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Action<T, TProp> set,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) {
			var prop = CreateProperty(propName, get, set, required, defaultValueHandling, defaultValue);
			contract.Properties.AddProperty(prop);
			return this;
		}

		public JsonObjectContractBuilder<T> AddCreatorArg<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) {
			var prop = CreateProperty(propName, get, required, defaultValueHandling, defaultValue);
			contract.Properties.AddProperty(prop);
			contract.CreatorParameters.AddProperty(prop);
			return this;
		}

		public JsonObjectContractBuilder<T> AddCreatorArg<TProp>
		(String propName,
		 Func<T, TProp> get,
		 Action<T, TProp> set,
		 Required required = Required.Always,
		 DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
		 Object? defaultValue = null) {
			var prop = CreateProperty(propName, get, set, required, defaultValueHandling, defaultValue);
			contract.Properties.AddProperty(prop);
			contract.CreatorParameters.AddProperty(prop);
			return this;
		}

		public JsonObjectContractBuilder<T> AddCreator<T1, T2> (Creator<T1, T2> creator) {
			contract.OverrideCreator = args => creator((T1) args[0]!, (T2) args[1]!);
			return this;
		}

		public JsonObjectContractBuilder<T> AddCreator<T1, T2, T3> (Creator<T1, T2, T3> creator) {
			contract.OverrideCreator = args => creator((T1) args[0]!, (T2) args[1]!, (T3) args[2]!);
			return this;
		}

		public JsonObjectContract Build () => contract;
	}
}
