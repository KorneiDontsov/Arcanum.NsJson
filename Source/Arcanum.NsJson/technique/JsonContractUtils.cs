// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;

	static class JsonContractUtils {
		public static Boolean IsOfAbstractClass (this JsonContract contract) =>
			contract.UnderlyingType.IsClass && contract.UnderlyingType.IsAbstract;

		public static Boolean IsOfNonAbstractClass (this JsonContract contract) =>
			contract.UnderlyingType.IsClass && ! contract.UnderlyingType.IsAbstract;

		public static Boolean HasNoConverter (this JsonContract contract) =>
			contract.Converter is null;

		#region creation
		public static JsonStringContract Copy (this JsonStringContract source) =>
			new JsonStringContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};

		public static JsonPrimitiveContract Copy (this JsonPrimitiveContract source) =>
			new JsonPrimitiveContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};

		public static JsonObjectContract Copy (this JsonObjectContract source) =>
			new JsonObjectContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				ExtensionDataGetter = source.ExtensionDataGetter,
				ExtensionDataNameResolver = source.ExtensionDataNameResolver,
				ExtensionDataSetter = source.ExtensionDataSetter,
				ExtensionDataValueType = source.ExtensionDataValueType,
				ItemNullValueHandling = source.ItemNullValueHandling,
				ItemRequired = source.ItemRequired,
				MemberSerialization = source.MemberSerialization,
				MissingMemberHandling = source.MissingMemberHandling,
				OverrideCreator = source.OverrideCreator
			};

		public static JsonArrayContract Copy (this JsonArrayContract source) =>
			new JsonArrayContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				HasParameterizedCreator = source.HasParameterizedCreator,
				OverrideCreator = source.OverrideCreator
			};

		public static JsonDictionaryContract Copy (this JsonDictionaryContract source) =>
			new JsonDictionaryContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				DictionaryKeyResolver = source.DictionaryKeyResolver,
				HasParameterizedCreator = source.HasParameterizedCreator,
				OverrideCreator = source.OverrideCreator
			};

		public static JsonDynamicContract Copy (this JsonDynamicContract source) =>
			new JsonDynamicContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				PropertyNameResolver = source.PropertyNameResolver
			};

		public static JsonISerializableContract Copy (this JsonISerializableContract source) =>
			new JsonISerializableContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				ISerializableCreator = source.ISerializableCreator
			};

		public static JsonLinqContract Copy (this JsonLinqContract source) =>
			new JsonLinqContract(source.UnderlyingType) {
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};

		static Dictionary<Type, Func<JsonContract, JsonContract>> copyJsonContractFunctorDict { get; }
			= new Dictionary<Type, Func<JsonContract, JsonContract>> {
				[typeof(JsonStringContract)] = source => Copy((JsonStringContract) source),
				[typeof(JsonPrimitiveContract)] = source => Copy((JsonPrimitiveContract) source),
				[typeof(JsonObjectContract)] = source => Copy((JsonObjectContract) source),
				[typeof(JsonArrayContract)] = source => Copy((JsonArrayContract) source),
				[typeof(JsonDictionaryContract)] = source => Copy((JsonDictionaryContract) source),
				[typeof(JsonDynamicContract)] = source => Copy((JsonDynamicContract) source),
				[typeof(JsonISerializableContract)] = source => Copy((JsonISerializableContract) source),
				[typeof(JsonLinqContract)] = source => Copy((JsonLinqContract) source)
			};

		public static JsonContract Copy (this JsonContract source) =>
			copyJsonContractFunctorDict.TryGetValue(source.GetType(), out var copyFunctor)
				? copyFunctor(source)
				: throw new Exception($"Argument '{nameof(source)}' is of unexpected type {source.GetType()}.");

		public static JsonContract WithConverter (this JsonContract source, JsonConverter converter) {
			var newContract = source.Copy();
			newContract.Converter = converter;
			return newContract;
		}
		#endregion
	}
}
