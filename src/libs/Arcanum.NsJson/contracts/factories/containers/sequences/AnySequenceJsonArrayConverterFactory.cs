// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using static RuntimeModule;

	public sealed class AnySequenceJsonArrayConverterFactory: IJsonArrayConverterFactory {
		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request, JsonArrayAttribute? jsonArrayAttribute) {
			if(request.dataType.HasOpenGenericInterface(typeof(IEnumerable<>), out var itemType)) {
				var converter =
					isJit
						? typeof(JitSequenceToJsonConverter<>)
							.MakeGenericType(itemType)
							.ConstructAs<IToJsonConverter>()
						: AotSequenceToJsonConverter.shared;
				request.ReturnWriteOnly(converter);
			}
		}
	}
}
