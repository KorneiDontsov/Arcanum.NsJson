// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;

static class ActivatorFunctions {
	public static Object Construct (this Type type) =>
		Activator.CreateInstance(type);

	public static T ConstructAs<T> (this Type type) where T: class =>
		(T) Activator.CreateInstance(type);

	public static Object Construct (this Type type, params Object[] @params) =>
		type.GetConstructor(@params.Select(a => a.GetType()).ToArray())?.Invoke(@params)
		?? throw new Exception($"Failed to find in type {type} constructor which can accept specified parameters.");

	public static T ConstructAs<T> (this Type type, params Object[] @params) =>
		(T) type.Construct(@params);
}
