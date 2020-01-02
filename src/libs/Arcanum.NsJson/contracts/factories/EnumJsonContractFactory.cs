// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;

	public sealed class EnumJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if (request.dataType.IsEnum) {
				var converter = new StringEnumConverter();
				request.Return(new JsonStringContract(request.dataType) { Converter = converter });
			}
		}
	}
}
