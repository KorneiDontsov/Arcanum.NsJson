// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Ext.AspNet {
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Formatters;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using System;

	public sealed class MvcJsonOptions {
		/// <summary>
		///     Gets or sets a flag to determine whether error messages from JSON deserialization by the
		///     <see cref = "NewtonsoftJsonInputFormatter" /> will be added to the
		///     <see cref = "ModelStateDictionary" />. If <see langword = "false" />, a generic error message
		///     will be used instead.
		/// </summary>
		/// <value>
		///     The default value is <see langword = "false" />.
		/// </value>
		/// <remarks>
		///     Error messages in the <see cref = "ModelStateDictionary" /> are often communicated to clients,
		///     either in HTML or using <see cref = "BadRequestObjectResult" />. In effect, this setting
		///     controls whether clients can receive detailed error messages about submitted JSON data.
		/// </remarks>
		public Boolean allowInputFormatterExceptionMessages { get; set; } = false;
	}
}
