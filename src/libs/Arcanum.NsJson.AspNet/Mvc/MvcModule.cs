// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet {
	using Arcanum.NsJson.Ext.AspNet;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using static Arcanum.NsJson.Module;

	public static class MvcModule {
		class StubContractResolver: IContractResolver {
			JsonConverter jsonConverter { get; }

			public StubContractResolver (JsonConverter jsonConverter) =>
				this.jsonConverter = jsonConverter;

			/// <inheritdoc />
			public JsonContract ResolveContract (Type type) =>
				new JsonLinqContract(type) { Converter = jsonConverter };
		}

		class JsonSerializerProxy: JsonConverter {
			IJsonSerializer jsonSerializer { get; }

			public JsonSerializerProxy (IJsonSerializer jsonSerializer) =>
				this.jsonSerializer = jsonSerializer;

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) => true;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) =>
				jsonSerializer.Write(writer, value);

			/// <inheritdoc />
			public override Object? ReadJson
				(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) =>
				jsonSerializer.MayRead(reader, objectType);
		}

		public static IMvcBuilder AddJson
			(this IMvcBuilder mvcBuilder, IJsonSerializer serializer, MvcJsonOptions options) =>
			mvcBuilder.AddNewtonsoftJson(
				nsOptions => {
					nsOptions.AllowInputFormatterExceptionMessages = options.allowInputFormatterExceptionMessages;
					nsOptions.SerializerSettings.ContractResolver =
						new StubContractResolver(new JsonSerializerProxy(serializer));
				});

		public static IMvcBuilder AddJson (this IMvcBuilder mvcBuilder, MvcJsonOptions options) =>
			mvcBuilder.AddJson(standardJsonSerializer, options);

		public static MvcJsonOptions standardMvcJsonOptions = new MvcJsonOptions();

		public static IMvcBuilder AddJson (this IMvcBuilder mvcBuilder) =>
			mvcBuilder.AddJson(standardMvcJsonOptions);
	}
}
