// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;

namespace Arcanum.ForNewtonsoftJson
{
	public static class JsonContractUtils
	{
		private static Dictionary<Type, Func<JsonContract, JsonContract>> _copyJsonContractFunctorDict { get; }
			= new Dictionary<Type, Func<JsonContract, JsonContract>>
			{
				[typeof(JsonStringContract)] = source => Copy((JsonStringContract)source),
				[typeof(JsonPrimitiveContract)] = source => Copy((JsonPrimitiveContract)source),
				[typeof(JsonObjectContract)] = source => Copy((JsonObjectContract)source),
				[typeof(JsonArrayContract)] = source => Copy((JsonArrayContract)source),
				[typeof(JsonDictionaryContract)] = source => Copy((JsonDictionaryContract)source),
				[typeof(JsonDynamicContract)] = source => Copy((JsonDynamicContract)source),
				[typeof(JsonISerializableContract)] = source => Copy((JsonISerializableContract)source),
				[typeof(JsonLinqContract)] = source => Copy((JsonLinqContract)source),
			};

		public static JsonContract Copy (this JsonContract source)
		{
			if (_copyJsonContractFunctorDict.TryGetValue(source.GetType(), out var copyFunctor))
			{
				return copyFunctor(source);
			}
			else
			{
				throw new Exception($"Argument '{nameof(source)}' is of unexpected type {source.GetType()}.");
			}
		}

		public static JsonStringContract Copy (this JsonStringContract source)
		{
			return new JsonStringContract(source.UnderlyingType)
			{
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};
		}

		public static JsonPrimitiveContract Copy (this JsonPrimitiveContract source)
		{
			return new JsonPrimitiveContract(source.UnderlyingType)
			{
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};
		}

		public static JsonObjectContract Copy (this JsonObjectContract source)
		{
			return new JsonObjectContract(source.UnderlyingType)
			{
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
		}

		public static JsonArrayContract Copy (this JsonArrayContract source)
		{
			return new JsonArrayContract(source.UnderlyingType)
			{
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
		}

		public static JsonDictionaryContract Copy (this JsonDictionaryContract source)
		{
			return new JsonDictionaryContract(source.UnderlyingType)
			{
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
		}

		public static JsonDynamicContract Copy (this JsonDynamicContract source)
		{
			return new JsonDynamicContract(source.UnderlyingType)
			{
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference,
				ItemConverter = source.ItemConverter,
				ItemIsReference = source.ItemIsReference,
				ItemReferenceLoopHandling = source.ItemReferenceLoopHandling,
				ItemTypeNameHandling = source.ItemTypeNameHandling,
				PropertyNameResolver = source.PropertyNameResolver,
			};
		}

		public static JsonISerializableContract Copy (this JsonISerializableContract source)
		{
			return new JsonISerializableContract(source.UnderlyingType)
			{
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
		}

		public static JsonLinqContract Copy (this JsonLinqContract source)
		{
			return new JsonLinqContract(source.UnderlyingType)
			{
				Converter = source.Converter,
				CreatedType = source.CreatedType,
				DefaultCreator = source.DefaultCreator,
				DefaultCreatorNonPublic = source.DefaultCreatorNonPublic,
				IsReference = source.IsReference
			};
		}
	}
}
