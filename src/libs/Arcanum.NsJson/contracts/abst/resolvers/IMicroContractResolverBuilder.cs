// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;

	public interface IMicroContractResolverBuilder {
		IMicroContractResolverBuilder AddContractFactory (IJsonContractFactory contractFactory);

		IMicroContractResolverBuilder AddConverterFactory (IJsonConverterFactory converterFactory);

		IMicroContractResolverBuilder AddMiddlewareFactory (IJsonMiddlewareFactory middlewareFactory);

		IContractResolver Build ();
	}
}
