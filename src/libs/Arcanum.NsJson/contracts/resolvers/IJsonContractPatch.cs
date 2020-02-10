// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json.Serialization;

	public interface IJsonContractPatch {
		/// <exception cref = "JsonContractException" />
		void Patch (JsonContract contract);
	}
}
