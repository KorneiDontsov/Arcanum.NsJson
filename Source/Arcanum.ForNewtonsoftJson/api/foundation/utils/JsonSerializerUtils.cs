// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	public static class JsonSerializerUtils
	{
		public static Object? DeserializeWithoutMiddleware (
			this JsonSerializer serializer,
			JsonReader reader,
			Type objectType
		)
		{
			using (ArcanumJsonContractResolver.ResolveContractArgs.Set(withoutMiddleware: true))
			{
				return serializer.Deserialize(reader, objectType);
			}
		}
	}
}
