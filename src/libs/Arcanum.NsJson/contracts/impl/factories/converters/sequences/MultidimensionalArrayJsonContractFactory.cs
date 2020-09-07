// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	// TODO: Multidimensional arrays do not have custom serialization yet.
	class MultidimensionalArrayJsonContractFactory: IJsonContractFactory {
		readonly NsJsonBasedContractFactory standardJsonContractFactory;

		public MultidimensionalArrayJsonContractFactory (NsJsonBasedContractFactory standardJsonContractFactory) =>
			this.standardJsonContractFactory = standardJsonContractFactory;

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if(request.dataType.IsArray && request.dataType.GetArrayRank() > 1)
				standardJsonContractFactory.Handle(request);
		}
	}
}
