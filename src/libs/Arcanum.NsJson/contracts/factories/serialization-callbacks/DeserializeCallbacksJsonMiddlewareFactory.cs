// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Runtime.Serialization;
	using static RuntimeModule;
	using static Arcanum.NsJson.Contracts.SerializationCallbackFunctions;

	public sealed class DeserializeCallbacksJsonMiddlewareFactory: IJsonMiddlewareFactory {
		class OnDeserializedFromJsonMiddleware: IFromJsonMiddleware {
			Action<Object> onDeserialized { get; }

			public OnDeserializedFromJsonMiddleware (Action<Object> onDeserialized) =>
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
			var callbacks = GetCallbacks(request.dataType, typeof(OnDeserializedAttribute));
			if(callbacks.Length > 0) {
				var onDeserialized =
					isJit
						? CompileComposedCallback(request.dataType, callbacks, $"OnDeserialized<{request.dataType}>")
						: ComposeCallback(callbacks);
				request.Yield(new OnDeserializedFromJsonMiddleware(onDeserialized));
			}
		}
	}
}
