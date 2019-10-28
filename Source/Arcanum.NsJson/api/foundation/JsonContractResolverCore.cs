// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;
	using System;

	class JsonContractResolverCore: IContractResolver {
		class Impl: DefaultContractResolver {
			public Impl () =>
				SerializeCompilerGeneratedMembers = true;

			public new JsonContract ResolveContract (Type dataType) =>
				base.CreateContract(dataType);
		}

		Impl impl { get; } = new Impl();

		/// <inheritdoc />
		public JsonContract ResolveContract (Type dataType) => impl.ResolveContract(dataType);
	}
}
