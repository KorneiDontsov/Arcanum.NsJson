// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	class AotCollectionMethods {
		[Preserve]
		// ReSharper disable once UnusedMember.Local
		static void Preserve (ICollection<Object> collection) {
			_ = collection.IsReadOnly;
			_ = collection.Count;
			collection.Add(new Object());
			collection.CopyTo(new Object[1], 0);
		}

		public MethodInfo isReadOnly { get; }
		public MethodInfo count { get; }
		public MethodInfo add { get; }
		public MethodInfo copyTo { get; }

		public AotCollectionMethods (MethodInfo isReadOnly, MethodInfo count, MethodInfo add, MethodInfo copyTo) {
			this.isReadOnly = isReadOnly;
			this.count = count;
			this.add = add;
			this.copyTo = copyTo;
		}

		public static Func<Type, AotCollectionMethods> forItemType { get; } =
			MemoizeFunc.Create(
				(Type itemType) => {
					var t = typeof(ICollection<>).MakeGenericType(itemType);
					// ReSharper disable once PossibleNullReferenceException
					var isReadOnly = t.GetProperty(nameof(ICollection<Object>.IsReadOnly))!.GetGetMethod();
					var count = t.GetProperty(nameof(ICollection<Object>.Count))!.GetGetMethod();
					var add = t.GetMethod(nameof(ICollection<Object>.Add))!;
					var copyTo = t.GetMethod(nameof(ICollection<Object>.CopyTo))!;
					return new AotCollectionMethods(isReadOnly, count, add, copyTo);
				});
	}
}
