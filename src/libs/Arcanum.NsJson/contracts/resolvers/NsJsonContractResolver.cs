// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class NsJsonContractResolver: IContractResolver {
		DefaultContractResolver decorated { get; } =
			new DefaultContractResolver {
				SerializeCompilerGeneratedMembers = true
			};

		public static NsJsonContractResolver shared { get; } = new NsJsonContractResolver();

		/// <inheritdoc />
		public JsonContract ResolveContract (Type dataType) => decorated.ResolveContract(dataType);
	}
}
