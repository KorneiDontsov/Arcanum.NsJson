// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Abstractions {
	using Newtonsoft.Json;
	using System;

	public sealed class JsonContractException: JsonException {
		public JsonContractException (String message, Exception? innerException = null)
			: base(message, innerException) { }
	}
}
