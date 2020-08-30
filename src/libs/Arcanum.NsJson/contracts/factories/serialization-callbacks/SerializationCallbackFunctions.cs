// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.Serialization;

	static class SerializationCallbackFunctions {
		public static ImmutableArray<MethodInfo> GetCallbacks (Type dataType, Type attributeType) {
			var result = ImmutableArray.CreateBuilder<MethodInfo>();
			for(var t = dataType; t is {}; t = t.BaseType)
				foreach(var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					if(m.DeclaringType == t && Attribute.IsDefined(m, attributeType)) {
						var @params = m.GetParameters();
						if(@params.Length != 1 || @params[0].ParameterType != typeof(StreamingContext))
							throw new JsonContractException($"Method {m} must accept only {typeof(StreamingContext)}.");
						else
							result.Add(m);
					}
			result.Reverse();
			return result.ToImmutable();
		}

		static Expression defaultContextExp { get; } =
			Expression.Constant(default(StreamingContext), typeof(StreamingContext));

		public static Action<Object> CompileComposedCallback
			(Type dataType, ImmutableArray<MethodInfo> callbacks, String callbackName) {
			var targetParam = Expression.Parameter(typeof(Object), "target");

			var targetExp = Expression.Convert(targetParam, dataType);

			var callbackCallExps = new List<MethodCallExpression>();
			foreach(var callback in callbacks) {
				var callExp = Expression.Call(targetExp, callback, defaultContextExp);
				callbackCallExps.Add(callExp);
			}
			var bodyExp = Expression.Block(callbackCallExps);

			return Expression.Lambda<Action<Object>>(bodyExp, callbackName, new[] { targetParam }).Compile();
		}

		static Object[] defaultContextAsObjArray { get; } = { default(StreamingContext)! };

		public static Action<Object> ComposeCallback (ImmutableArray<MethodInfo> callbacks) =>
			target => {
				foreach(var callback in callbacks)
					callback.Invoke(target, defaultContextAsObjArray);
			};
	}
}
