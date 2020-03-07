// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Concurrent;
	using System.Runtime.Serialization;
	using System.Threading;

	class ArcaneJsonSerializer: IJsonSerializer {
		public JsonSerializer nsSerializer { get; }

		/// <inheritdoc />
		public void Write (JsonWriter jsonWriter, Object? maybeData) =>
			nsSerializer.Serialize(jsonWriter, maybeData);

		/// <inheritdoc />
		public Object? MayRead (JsonReader jsonReader, Type dataType) =>
			nsSerializer.Deserialize(jsonReader, dataType);

		public JsonSerializerSetup setup { get; }

		static ConcurrentBag<JsonSerializerContext> contextPool { get; } =
			new ConcurrentBag<JsonSerializerContext>();

		ConcurrentDictionary<Int32, JsonSerializerContext> contexts { get; } =
			new ConcurrentDictionary<Int32, JsonSerializerContext>();

		Func<Int32, JsonSerializerContext> createContext { get; }

		Action<Int32> closeContext { get; }

		JsonSerializerContext CreateContext (Int32 threadId) {
			var context = contextPool.TryTake(out var pooledContext) ? pooledContext : new JsonSerializerContext();
			context.threadId = threadId;
			context.closeContext = closeContext;
			return context;
		}

		void CloseContext (Int32 threadId) {
			if (contexts.TryRemove(threadId, out var context)) {
				context.threadId = 0;
				context.closeContext = null!;
				contextPool.Add(context);
			}
		}

		public JsonSerializerContext CaptureContext () {
			var context = contexts.GetOrAdd(Thread.CurrentThread.ManagedThreadId, createContext);
			context.Capture();
			return context;
		}

		public ArcaneJsonSerializer (JsonSerializerSetup setup, IContractResolver contractResolver) {
			nsSerializer =
				new JsonSerializer {
					// important for internal impl
					ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
					// design
					ObjectCreationHandling = ObjectCreationHandling.Replace,
					ConstructorHandling = ConstructorHandling.Default,
					MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					NullValueHandling = NullValueHandling.Include,
					DefaultValueHandling = DefaultValueHandling.Include,
					PreserveReferencesHandling = PreserveReferencesHandling.None,
					StringEscapeHandling = StringEscapeHandling.Default,
					FloatFormatHandling = FloatFormatHandling.String,
					FloatParseHandling = FloatParseHandling.Decimal,
					DateFormatHandling = DateFormatHandling.IsoDateFormat,
					DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
					DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
					DateParseHandling = DateParseHandling.DateTimeOffset,
					TypeNameHandling = TypeNameHandling.None,
					Culture = setup.culture,
					MaxDepth =
						setup.maxDepth switch {
							0 => (Int32?) null,
							var d when d <= Int32.MaxValue => (Int32) d,
							_ => Int32.MaxValue
						},
					CheckAdditionalContent = setup.checkAdditionalContent,
					Context = new StreamingContext(0, this),
					ContractResolver = contractResolver
				};
			this.setup = setup;
			createContext = CreateContext;
			closeContext = CloseContext;
		}
	}
}
