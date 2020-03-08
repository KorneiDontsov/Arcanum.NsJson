// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;
	using System;

	public sealed class NsJsonBasedContractFactory: IJsonContractFactory {
		class Resolver: DefaultContractResolver {
			public Resolver () =>
				SerializeCompilerGeneratedMembers = true;

			public new JsonContract CreateContract (Type dataType) =>
				base.CreateContract(dataType);
		}

		static Resolver resolver { get; } = new Resolver();

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			var contract = resolver.CreateContract(request.dataType);
			contract.OnDeserializingCallbacks.Clear();
			contract.OnDeserializedCallbacks.Clear();
			contract.OnErrorCallbacks.Clear();
			contract.IsReference = false;
			request.Return(contract);
		}
	}
}
