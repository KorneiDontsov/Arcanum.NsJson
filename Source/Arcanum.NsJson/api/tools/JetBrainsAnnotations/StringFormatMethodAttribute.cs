// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

// ReSharper disable All

using System;

namespace Arcanum.NsJson.Annotations {
	/// <inheritdoc />
	/// <summary>
	///     Indicates that the marked method builds string by the format pattern and (optional) arguments.
	///     The parameter, which contains the format string, should be given in constructor. The format string
	///     should be in <see cref = "String.Format(IFormatProvider,String,System.Object[])" />-like form.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Constructor
		| AttributeTargets.Method
		| AttributeTargets.Property
		| AttributeTargets.Delegate)]
	public sealed class StringFormatMethodAttribute: Attribute {
		public String FormatParameterName { get; }

		/// <inheritdoc />
		/// <param name = "formatParameterName">
		///     Specifies which parameter of an annotated method should be treated as the format string
		/// </param>
		public StringFormatMethodAttribute (String formatParameterName) => FormatParameterName = formatParameterName;
	}
}
