﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	public interface IJsonContractFactory {
		/// <exception cref = "JsonContractException" />
		void Handle (IJsonContractRequest request);
	}
}
