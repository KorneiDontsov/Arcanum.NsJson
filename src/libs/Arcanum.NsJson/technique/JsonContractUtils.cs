// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json.Serialization;
	using System;

	static class JsonContractUtils {
		public static Boolean IsOfAbstractClass (this JsonContract contract) =>
			contract.UnderlyingType.IsClass && contract.UnderlyingType.IsAbstract;

		public static Boolean IsOfNonAbstractClass (this JsonContract contract) =>
			contract.UnderlyingType.IsClass && ! contract.UnderlyingType.IsAbstract;
	}
}
