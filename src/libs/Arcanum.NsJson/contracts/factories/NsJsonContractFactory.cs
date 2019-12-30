// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class NsJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public JsonContract? MayCreateContract (Type dataType) =>
			NsJsonContractResolver.shared.ResolveContract(dataType);
	}
}
