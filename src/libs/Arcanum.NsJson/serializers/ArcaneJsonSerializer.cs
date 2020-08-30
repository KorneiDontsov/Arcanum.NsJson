// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Runtime.CompilerServices;
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

		ConditionalWeakTable<Thread, JsonSerializationContext> contexts { get; } =
			new ConditionalWeakTable<Thread, JsonSerializationContext>();

		public LocalsCollectionOwner CaptureLocals () {
			var context = contexts.GetOrCreateValue(Thread.CurrentThread);
			var locals = LocalsCollection.Capture(context);
			return new LocalsCollectionOwner(locals);
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
							0 => null,
							var d when d <= Int32.MaxValue => (Int32) d,
							_ => Int32.MaxValue
						},
					CheckAdditionalContent = setup.checkAdditionalContent,
					Context = new StreamingContext(0, this),
					ContractResolver = contractResolver
				};
			this.setup = setup;
		}
	}
}
