// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Diagnostics.CodeAnalysis;

	public sealed class BasicJsonContractFactory: IJsonContractFactory {
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		class Resolver: DefaultContractResolver {
			public Resolver () =>
				SerializeCompilerGeneratedMembers = true;

			public new JsonContract CreateContract (Type dataType) =>
				base.CreateContract(dataType);
		}

		static Resolver resolver { get; } = new Resolver();

		internal static JsonContract CreateContract (Type dataType) => resolver.CreateContract(dataType);

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) =>
			request.Return(resolver.CreateContract(request.dataType));
	}
}
