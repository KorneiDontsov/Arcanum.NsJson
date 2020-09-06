// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	public static class JsonConverterRequestFunctions {
		public static void Return (this IJsonConverterRequest jsonConverterRequest, IJsonConverter jsonConverter) =>
			jsonConverterRequest.Return(toJsonConverter: jsonConverter, fromJsonConverter: jsonConverter);
	}
}
