// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet {
	using Arcanum.NsJson.Ext.AspNet;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json.Serialization;
	using static Arcanum.NsJson.Module;

	public static class MvcModule {
		public static MvcJsonOptions standardMvcJsonOptions = new MvcJsonOptions();

		public static IMvcBuilder AddArcaneNsJson
		(this IMvcBuilder mvcBuilder,
		 JsonSerializerSetup serializerSetup,
		 IContractResolver contractResolver,
		 MvcJsonOptions options,
		 JsonSerializerConfig? config = null) =>
			mvcBuilder.AddNewtonsoftJson(
				nsOptions => {
					nsOptions.AllowInputFormatterExceptionMessages = options.allowInputFormatterExceptionMessages;
					var settings = nsOptions.SerializerSettings;
					serializerSetup.SetUpNsSerializerSettings(settings, config);
					settings.ContractResolver = contractResolver;
				});

		public static IMvcBuilder AddArcaneNsJson
		(this IMvcBuilder mvcBuilder,
		 MvcJsonOptions options,
		 JsonSerializerConfig? config = null) =>
			mvcBuilder.AddArcaneNsJson(standardJsonSerializerSetup, standardContractResolver, options, config);

		public static IMvcBuilder AddArcaneNsJson (this IMvcBuilder mvcBuilder, JsonSerializerConfig? config = null) =>
			mvcBuilder.AddArcaneNsJson(standardMvcJsonOptions, config);
	}
}
