// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Tests.Arcanum.ForNewtonsoftJson {
	using global::Arcanum.ForNewtonsoftJson;
	using Newtonsoft.Json;

	public abstract class ArcanumJsonContractResolverTest {
		protected JsonSerializer serializer { get; }

		protected ArcanumJsonContractResolverTest () =>
			serializer = new JsonSerializer { ContractResolver = new ArcanumJsonContractResolver() };
	}
}
