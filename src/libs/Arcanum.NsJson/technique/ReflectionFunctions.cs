// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;
	using System.Collections.Immutable;
	using System.Reflection;

	static class ReflectionFunctions {
		public static TAttr? MatchCustomAttribute<TAttr> (this ICustomAttributeProvider type, Boolean inherit = true)
		where TAttr: class {
			foreach (var attribute in type.GetCustomAttributes(inherit))
				if (attribute is TAttr matched)
					return matched;

			return null;
		}

		public static ImmutableArray<TAttr> MatchCustomAttributes<TAttr>
		(this ICustomAttributeProvider type,
		 Boolean inherit = true)
		where TAttr: class {
			var results = ImmutableArray.CreateBuilder<TAttr>();
			foreach (var attribute in type.GetCustomAttributes(inherit))
				if (attribute is TAttr matched)
					results.Add(matched);
			return results.ToImmutable();
		}
	}
}
