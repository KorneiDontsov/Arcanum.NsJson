// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Runtime.Serialization;
	using static Arcanum.NsJson.Contracts.SerializationCallbackFunctions;

	public sealed class SerializeCallbacksJsonMiddlewareFactory: IJsonMiddlewareFactory {
		class OnSerializingToJsonMiddleware: IToJsonMiddleware {
			Action<Object> onSerializing { get; }

			public OnSerializingToJsonMiddleware (Action<Object> onSerializing) =>
				this.onSerializing = onSerializing;

			/// <inheritdoc />
			public void Write
				(IJsonSerializer serializer,
				 JsonWriter writer,
				 Object value,
				 WriteJson previous,
				 ILocalsCollection locals) {
				onSerializing(value);
				previous(serializer, writer, value);
			}
		}

		class OnSerializedToJsonMiddleware: IToJsonMiddleware {
			Action<Object> onSerialized { get; }

			public OnSerializedToJsonMiddleware (Action<Object> onSerialized) =>
				this.onSerialized = onSerialized;

			/// <inheritdoc />
			public void Write
				(IJsonSerializer serializer,
				 JsonWriter writer,
				 Object value,
				 WriteJson previous,
				 ILocalsCollection locals) {
				previous(serializer, writer, value);
				onSerialized(value);
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonMiddlewareRequest request) {
			var onSerializingCallbacks = GetCallbacks(request.dataType, typeof(OnSerializingAttribute));
			if(onSerializingCallbacks.Length > 0) {
				var onSerializing =
					RuntimeModule.isJit
						? CompileComposedCallback(
							request.dataType,
							onSerializingCallbacks,
							$"OnSerializing<{request.dataType}>")
						: ComposeCallback(onSerializingCallbacks);
				request.Yield(new OnSerializingToJsonMiddleware(onSerializing));
			}

			var onSerializedCallbacks = GetCallbacks(request.dataType, typeof(OnSerializedAttribute));
			if(onSerializedCallbacks.Length > 0) {
				var onSerialized =
					RuntimeModule.isJit
						? CompileComposedCallback(
							request.dataType,
							onSerializedCallbacks,
							$"OnSerialized<{request.dataType}>")
						: ComposeCallback(onSerializedCallbacks);
				request.Yield(new OnSerializedToJsonMiddleware(onSerialized));
			}
		}
	}
}
