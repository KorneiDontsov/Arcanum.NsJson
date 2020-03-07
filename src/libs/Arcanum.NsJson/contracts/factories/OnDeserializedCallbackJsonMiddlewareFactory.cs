// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Runtime.Serialization;
	using static RuntimeModule;

	public class OnDeserializedCallbackJsonMiddlewareFactory: IJsonMiddlewareFactory {
		static List<MethodInfo> GetCallbacks (Type dataType) {
			var result = new List<MethodInfo>();
			for (var t = dataType; t is {}; t = t.BaseType)
				foreach (var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					if (m.DeclaringType == t && Attribute.IsDefined(m, typeof(OnDeserializedAttribute))) {
						var @params = m.GetParameters();
						if (@params.Length != 1 || @params[0].ParameterType != typeof(StreamingContext))
							throw new JsonContractException($"Method {m} must accept only {typeof(StreamingContext)}.");
						else
							result.Add(m);
					}
			result.Reverse();
			return result;
		}

		static Expression defaultContextExp { get; } =
			Expression.Constant(default(StreamingContext), typeof(StreamingContext));

		static Action<Object> CompileComposedCallback (Type dataType, List<MethodInfo> callbacks) {
			var targetParam = Expression.Parameter(typeof(Object), "target");

			var targetExp = Expression.Convert(targetParam, dataType);

			var callbackCallExps = new List<MethodCallExpression>();
			foreach (var callback in callbacks) {
				var callExp = Expression.Call(targetExp, callback, defaultContextExp);
				callbackCallExps.Add(callExp);
			}
			var bodyExp = Expression.Block(callbackCallExps);

			var lambdaName = $"OnDeserialized<{dataType}>";
			return Expression.Lambda<Action<Object>>(bodyExp, lambdaName, new[] { targetParam }).Compile();
		}

		static Object[] defaultContextAsObjArray { get; } = { default(StreamingContext) };

		static Action<Object> ReflectComposedCallback (List<MethodInfo> callbacks) =>
			target => {
				foreach (var callback in callbacks)
					callback.Invoke(target, defaultContextAsObjArray);
			};

		class OnDeserializedCallbackJsonMiddleware: IFromJsonMiddleware {
			Action<Object> onDeserialized { get; }

			public OnDeserializedCallbackJsonMiddleware (Action<Object> onDeserialized) =>
				this.onDeserialized = onDeserialized;

			/// <inheritdoc />
			public Object Read
				(IJsonSerializer serializer, JsonReader reader, ReadJson next, ILocalsCollection locals) {
				var data = next(serializer, reader);
				onDeserialized(data);
				return data;
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonMiddlewareRequest request) {
			var callbacks = GetCallbacks(request.dataType);
			if (callbacks.Count > 0) {
				var onDeserialized =
					isJit
						? CompileComposedCallback(request.dataType, callbacks)
						: ReflectComposedCallback(callbacks);
				request.Yield(new OnDeserializedCallbackJsonMiddleware(onDeserialized));
			}
		}
	}
}
