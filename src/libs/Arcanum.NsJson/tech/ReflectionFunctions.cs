// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

static class ReflectionFunctions {
	public static MethodInfo GetGenericMethodDefinition
		(this Type type, String name,
		 BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public) {
		foreach(var method in type.GetMethods(bindingFlags))
			if(method.IsGenericMethodDefinition && method.Name == name)
				return method;
		var msg =
			$"Generic method definition '{name}' is not found in type {type} where binding flags are {bindingFlags}.";
		throw new Exception(msg);
	}

	public static Delegate CreateGenericDelegate
		(this MethodInfo methodInfo, Type delegateTypeDefinition, params Type[] delegateTypeParams) =>
		methodInfo.CreateDelegate(delegateTypeDefinition.MakeGenericType(delegateTypeParams));

	public static Type? MayMakeGenericType (Type definitionType, Type itemType) {
		try {
			return definitionType.MakeGenericType(itemType);
		}
		catch {
			return null;
		}
	}

	static readonly Func<(Type type, Type interfaceType), ImmutableList<Type>> getUnderlyingTypesOfGenericInterfaces =
		MemoizeFunc.Create(
			((Type type, Type interfaceType) args) => {
				if(! args.interfaceType.IsInterface)
					throw new Exception($"Type {args.interfaceType} is not interface.");
				else if(! args.interfaceType.IsGenericTypeDefinition)
					throw new Exception($"Interface type {args.interfaceType} is not open generic type.");
				else if(args.type.IsGenericTypeDefinition)
					throw new Exception(
						$"Type {args.type} is open generic type and cannot be matched to open generic type.");
				else
					return args.type.GetInterfaces()
						.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == args.interfaceType)
						.Select(i => i.GetGenericArguments()[0])
						.ToImmutableList();
			});

	public static Type? MaybeUnderlyingTypeIfHasGenericInterface (this Type type, Type interfaceType) {
		var underlyingTypes = getUnderlyingTypesOfGenericInterfaces((type, interfaceType));
		return underlyingTypes.IsEmpty ? null : underlyingTypes[0];
	}

	public static Boolean HasOpenGenericInterface
		(this Type type, Type interfaceType, [NotNullWhen(true)] out Type? underlyingType) {
		var underlyingTypes = getUnderlyingTypesOfGenericInterfaces((type, interfaceType));
		if(underlyingTypes.IsEmpty) {
			underlyingType = null;
			return false;
		}
		else {
			underlyingType = underlyingTypes[0];
			return true;
		}
	}

	public static Boolean HasOpenGenericInterface (this Type type, Type interfaceType) {
		var underlyingTypes = getUnderlyingTypesOfGenericInterfaces((type, interfaceType));
		return ! underlyingTypes.IsEmpty;
	}

	public static ImmutableList<Type> GetUnderlyingTypesOfClosedGenericInterfaces
		(this Type type, Type interfaceType) =>
		getUnderlyingTypesOfGenericInterfaces((type, interfaceType));
}
