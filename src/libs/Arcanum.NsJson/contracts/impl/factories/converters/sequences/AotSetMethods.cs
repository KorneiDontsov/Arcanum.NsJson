// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	class AotSetMethods {
		[Preserve, MethodImpl(MethodImplOptions.NoOptimization)]
		// ReSharper disable once UnusedMember.Local
		static void Preserve (ISet<Object> set) =>
			_ = set.Add(new Object());

		public MethodInfo add { get; }

		AotSetMethods (MethodInfo add) =>
			this.add = add;

		public static Func<Type, AotSetMethods> forItemType { get; } =
			MemoizeFunc.Create(
				(Type itemType) => {
					var t = typeof(ISet<>).MakeGenericType(itemType);
					var add = t.GetMethod(nameof(ISet<Object>.Add))!;
					return new AotSetMethods(add);
				});
	}
}
