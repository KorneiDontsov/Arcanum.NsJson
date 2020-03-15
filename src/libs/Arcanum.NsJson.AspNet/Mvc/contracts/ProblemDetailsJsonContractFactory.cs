// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Ext.AspNet {
	using Arcanum.NsJson.Contracts;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
	using Newtonsoft.Json.Serialization;

	class ProblemDetailsJsonContractFactory: IJsonContractFactory {
		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if(request.dataType == typeof(ProblemDetails))
				request.Return(
					new JsonLinqContract(typeof(ProblemDetails))
						{ Converter = new ProblemDetailsConverter() });
			else if(request.dataType == typeof(ValidationProblemDetails))
				request.Return(
					new JsonLinqContract(typeof(ValidationProblemDetails))
						{ Converter = new ValidationProblemDetailsConverter() });
		}
	}
}
