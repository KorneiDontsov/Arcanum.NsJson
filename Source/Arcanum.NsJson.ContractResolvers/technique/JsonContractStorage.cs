// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.ContractResolvers {
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Concurrent;

	class JsonContractStorage {
		Func<Type, JsonContract> createContract { get; }

		ConcurrentDictionary<Type, JsonContract> contractDict { get; }

		public JsonContractStorage (Func<Type, JsonContract> createContract) {
			this.createContract = createContract;
			contractDict = new ConcurrentDictionary<Type, JsonContract>();
		}

		public JsonContract GetOrCreate (Type dataType) => contractDict.GetOrAdd(dataType, createContract);
	}
}
