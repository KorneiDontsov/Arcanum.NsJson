// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.Contracts;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Immutable;
	using System.Globalization;
	using static Arcanum.NsJson.Contracts.Module;

	public static class Module {
		public static IJsonSerializer CreateJsonSerializer
			(this JsonSerializerSetup setup, IContractResolver contractResolver) =>
			new ArcaneJsonSerializer(setup, contractResolver);

		public static JsonSerializer CreateNsJsonSerializer
			(this JsonSerializerSetup setup, IContractResolver contractResolver) =>
			new ArcaneJsonSerializer(setup, contractResolver).nsSerializer;

		public static JsonSerializerSetup standardJsonSerializerSetup { get; } =
			new JsonSerializerSetup(
				CultureInfo.InvariantCulture,
				maxDepth: 32,
				checkAdditionalContent: false,
				extensions: ImmutableDictionary<Object, Object>.Empty);

		public static IJsonSerializer standardJsonSerializer { get; } =
			standardJsonSerializerSetup.CreateJsonSerializer(standardJsonContractResolver);
	}
}
