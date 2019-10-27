// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using DataContracts;
	using Newtonsoft.Json.Serialization;
	using System;

	class JsonContractResolverBase: DefaultContractResolver {
		protected IDataTypeInfoFactory dataTypeInfoFactory => DataTypeInfoStorage.shared;

		public JsonContractResolverBase () =>
			SerializeCompilerGeneratedMembers = true;

		JsonContract? MaybeUnionContract (JsonContract contract) {
			if (contract.IsOfAbstractClass() && contract.HasNoConverter()
			&& dataTypeInfoFactory.Get(contract.UnderlyingType).asUnionInfo is { } unionInfo) {
				if (contract.UnderlyingType != unionInfo.dataType)
					throw
						new Exception(
							$"'{nameof(unionInfo)}' {unionInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}.");
				else if (unionInfo.hasErrors)
					throw
						new JsonContractResolveException(
							$"Cannot resolve {unionInfo.dataType} because it has errors.\n{unionInfo.GetErrorString()}");
				else {
					contract.Converter = new UnionJsonConverter(unionInfo);
					return contract;
				}
			}
			else
				return null;
		}

		/// <inheritdoc />
		protected override sealed JsonContract CreateContract (Type objectType) {
			var contract = base.CreateContract(objectType);
			return
				MaybeUnionContract(contract)
				?? contract;
		}
	}
}
