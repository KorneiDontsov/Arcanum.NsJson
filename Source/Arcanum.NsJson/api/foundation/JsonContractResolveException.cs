// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;

	public sealed class JsonContractResolveException: JsonException {
		public JsonContractResolveException (String message, Exception? innerException = null)
			: base(message, innerException) { }
	}
}
