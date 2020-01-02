// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class EnumJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public JsonContract? MayCreateContract (Type dataType) =>
			dataType.IsEnum
				? new JsonStringContract(dataType) { Converter = new StringEnumConverter() }
				: null;
	}
}
