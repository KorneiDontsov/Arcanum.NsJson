// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;

	public sealed class EnumJsonContractPatch: IJsonContractPatch {
		/// <inheritdoc />
		public void Patch (JsonContract contract) {
			if (contract.UnderlyingType.IsEnum) contract.Converter ??= new StringEnumConverter();
		}
	}
}
