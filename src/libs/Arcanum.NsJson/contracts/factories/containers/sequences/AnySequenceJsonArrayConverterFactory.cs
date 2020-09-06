// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using static RuntimeModule;

	public sealed class AnySequenceJsonArrayConverterFactory: IJsonArrayConverterFactory {
		internal static IToJsonConverter CreateToJsonConverter (Type itemType) =>
			isJit
				? typeof(JitSequenceToJsonConverter<>)
					.MakeGenericType(itemType)
					.ConstructAs<IToJsonConverter>()
				: AotSequenceToJsonConverter.shared;

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(request.dataType.HasOpenGenericInterface(typeof(IEnumerable<>), out var itemType)) {
				var converter = CreateToJsonConverter(itemType);
				request.ReturnWriteOnly(converter);
			}
		}
	}
}
