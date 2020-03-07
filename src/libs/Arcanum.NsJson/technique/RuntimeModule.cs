// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Runtime.CompilerServices;

static class RuntimeModule {
	sealed class PreserveAttribute: Attribute { }

	struct AnyType { }

	interface IAnyTypeFactory {
		Object ConstructAnyValue ();
	}

	sealed class SomeTypeFactory<T>: IAnyTypeFactory {
		[Preserve]
		public SomeTypeFactory () { }

		[Preserve, MethodImpl(MethodImplOptions.NoOptimization)]
		public Object ConstructAnyValue () => Activator.CreateInstance<T>()!;
	}

	[MethodImpl(MethodImplOptions.NoOptimization)]
	static Type GetGenericTypeDefinition () =>
		typeof(SomeTypeFactory<>);

	[MethodImpl(MethodImplOptions.NoOptimization)]
	static Type GetAnyType () =>
		typeof(AnyType);

	[MethodImpl(MethodImplOptions.NoOptimization)]
	static Func<Type, Type, Type> GetClosedGenericTypeConstructor () =>
		(definition, arg) => definition.MakeGenericType(arg);

	public static Boolean isJit { get; set; }

	static RuntimeModule () {
		try {
			// Unity and Mono developers always tries to analyze a code that uses a reflection to keep code
			// used through reflection from non-compilation. So let's make sure that it will be a little harder
			// for them to understand which code is used here.

			var closedGenericType = GetClosedGenericTypeConstructor()(GetGenericTypeDefinition(), GetAnyType());
			var factory = (IAnyTypeFactory) Activator.CreateInstance(closedGenericType);
			_ = factory.ConstructAnyValue();

			isJit = true;
		}
		catch {
			isJit = false;
		}
	}
}
