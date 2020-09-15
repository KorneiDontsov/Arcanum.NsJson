// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Ext.AspNet {
	using Arcanum.NsJson.Contracts;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
	using Newtonsoft.Json.Serialization;

	class ProblemDetailsJsonContractCreator: IJsonContractCreator {
		/// <inheritdoc />
		public JsonContract CreateJsonContract () =>
			new JsonObjectContract(typeof(ProblemDetails)) { Converter = new ProblemDetailsConverter() };
	}
}
