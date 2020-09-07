// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	class AotCollectionMethods {
		[Preserve, MethodImpl(MethodImplOptions.NoOptimization)]
		// ReSharper disable once UnusedMember.Local
		static void Preserve (ICollection<Object> collection) {
			_ = collection.IsReadOnly;
			_ = collection.Count;
			collection.Add(new Object());
			collection.Remove(new Object());
			collection.CopyTo(new Object[1], 0);
		}

		public MethodInfo isReadOnly { get; }
		public MethodInfo count { get; }
		public MethodInfo add { get; }
		public MethodInfo remove { get; }
		public MethodInfo copyTo { get; }

		AotCollectionMethods
			(MethodInfo isReadOnly, MethodInfo count, MethodInfo add, MethodInfo remove, MethodInfo copyTo) {
			this.isReadOnly = isReadOnly;
			this.count = count;
			this.add = add;
			this.remove = remove;
			this.copyTo = copyTo;
		}

		public static Func<Type, AotCollectionMethods> forItemType { get; } =
			MemoizeFunc.Create(
				(Type itemType) => {
					var t = typeof(ICollection<>).MakeGenericType(itemType);
					var isReadOnly = t.GetProperty(nameof(ICollection<Object>.IsReadOnly))!.GetGetMethod();
					var count = t.GetProperty(nameof(ICollection<Object>.Count))!.GetGetMethod();
					var add = t.GetMethod(nameof(ICollection<Object>.Add))!;
					var remove = t.GetMethod(nameof(ICollection<Object>.Remove))!;
					var copyTo = t.GetMethod(nameof(ICollection<Object>.CopyTo))!;
					return new AotCollectionMethods(isReadOnly, count, add, remove, copyTo);
				});
	}
}
