﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.ContractModules;
	using Arcanum.NsJson.ContractResolvers;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public static class JsonFactory {
		static JsonSerializerSetup serializerSetup { get; }

		static IContractResolver contractResolver { get; }

		public static IJsonSerializer defaultSerializer { get; }

		static JsonFactory () {
			serializerSetup = JsonSerializerSetup.arcane;
			contractResolver =
				new DistributionBasedJsonContractResolver(
					core: NsJsonContractResolver.shared,
					jsonContractDistribution: JsonContractDistributionFactory.BuildArcane().Ok(),
					middlewareJsonContractDistribution: JsonContractDistributionFactory.BuildArcaneMiddleware().Ok());
			defaultSerializer =
				new JsonSerializerAdapter(
					serializer: serializerSetup.CreateNsSerializer(contractResolver));
		}

		public static JsonSerializer NsSerializer (JsonSerializerConfig? serializerConfig = null) =>
			serializerSetup.CreateNsSerializer(contractResolver, serializerConfig);

		public static IJsonSerializer Serializer (JsonSerializerConfig serializerConfig) =>
			new JsonSerializerAdapter(serializer: NsSerializer(serializerConfig));
	}
}
