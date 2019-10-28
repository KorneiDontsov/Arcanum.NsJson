// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Arcanum.NsJson.ContractResolvers;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;

	class EnumJsonContractModule: IJsonContractGenericModule {
		/// <inheritdoc />
		public JsonContract? MayCreateContract (JsonContract baseContract) {
			if (baseContract.UnderlyingType.IsEnum) {
				baseContract.Converter = new StringEnumConverter();
				return baseContract;
			}
			else
				return null;
		}
	}
}
