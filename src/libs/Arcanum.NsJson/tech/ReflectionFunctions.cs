// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
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

	static Func<(Type type, Type interfaceType), Type?> maybeUnderlyingTypeIfHasGenericInterface { get; } =
		MemoizeFunc.Create<(Type type, Type interfaceType), Type?>(
			(args, self) => {
				if(! args.interfaceType.IsInterface)
					throw new Exception($"Type {args.interfaceType} is not interface.");
				else if(! args.interfaceType.IsGenericTypeDefinition)
					throw new Exception($"Interface type {args.interfaceType} is not open generic type.");
				else if(args.type.IsGenericTypeDefinition) {
					var msg = $"Type {args.type} is open generic type and cannot be matched to open generic type.";
					throw new Exception(msg);
				}
				else {
					foreach(var interfaceType in args.type.GetInterfaces()) {
						var matched = interfaceType.IsGenericType
						              && interfaceType.GetGenericTypeDefinition() == args.interfaceType;
						if(matched) return interfaceType.GetGenericArguments()[0];
					}
					return null;
				}
			});

	public static Type? MaybeUnderlyingTypeIfHasGenericInterface (this Type type, Type interfaceType) =>
		maybeUnderlyingTypeIfHasGenericInterface((type, interfaceType));

	public static Boolean HasOpenGenericInterface
		(this Type type, Type interfaceType, [NotNullWhen(true)] out Type? underlyingType) {
		underlyingType = maybeUnderlyingTypeIfHasGenericInterface((type, interfaceType));
		return underlyingType is {};
	}

	public static Boolean HasOpenGenericInterface (this Type type, Type interfaceType) =>
		maybeUnderlyingTypeIfHasGenericInterface((type, interfaceType)) is {};
}
