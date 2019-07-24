// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;

using Newtonsoft.Json.Serialization;

namespace Arcanum.ForNewtonsoftJson
{
	public class ArcanumJsonContractResolver : DefaultContractResolver
	{
		private sealed class Immutable : IContractResolver
		{
			private ArcanumJsonContractResolver _core { get; }

			public Immutable ()
			{
				_core = new ArcanumJsonContractResolver();
			}

			/// <inheritdoc />
			public JsonContract ResolveContract (Type type) => _core.ResolveContract(type);
		}

		public static IContractResolver shared { get; } = new Immutable();
	}
}
