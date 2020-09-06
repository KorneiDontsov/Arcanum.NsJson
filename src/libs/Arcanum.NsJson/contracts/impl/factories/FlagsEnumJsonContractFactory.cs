// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class FlagsEnumJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if(request.dataType.IsEnum && Attribute.IsDefined(request.dataType, typeof(FlagsAttribute))) {
				var converter = new StringEnumConverter { AllowIntegerValues = false };
				request.Return(new JsonLinqContract(request.dataType) { Converter = converter });
			}
		}
	}
}
