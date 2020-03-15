// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.Contracts;
	using Newtonsoft.Json;

	static class NsJsonSerializerFunctions {
		public static ArcaneJsonSerializer Arcane (this JsonSerializer jsonSerializer) =>
			jsonSerializer.Context.Context is ArcaneJsonSerializer arcaneJsonSerializer
				? arcaneJsonSerializer
				: throw new JsonException("Json serializer is not arcane.");

		public static ArcaneJsonSerializer Arcane (this IJsonSerializer jsonSerializer) =>
			jsonSerializer is ArcaneJsonSerializer arcaneJsonSerializer
				? arcaneJsonSerializer
				: throw new JsonException("Json serializer is not arcane.");
	}
}
