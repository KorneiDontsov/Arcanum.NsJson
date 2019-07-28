// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	public sealed class JsonContractResolveException : JsonException
	{
		public JsonContractResolveException (String message, Exception? innerException = null)
		: base(message, innerException) { }
	}
}
