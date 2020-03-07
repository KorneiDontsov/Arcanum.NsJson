// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;

	public class DiscardDeserializationCallbacksJsonContractPatch: IJsonContractPatch {
		/// <inheritdoc />
		public void Patch (JsonContract contract) {
			contract.OnDeserializingCallbacks.Clear();
			contract.OnDeserializedCallbacks.Clear();
		}
	}
}
