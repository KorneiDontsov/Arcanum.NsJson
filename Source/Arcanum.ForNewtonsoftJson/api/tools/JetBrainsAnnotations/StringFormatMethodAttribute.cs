/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

// ReSharper disable All

using System;

namespace Arcanum.ForNewtonsoftJson.Annotations
{
	/// <inheritdoc />
	/// <summary>
	///     Indicates that the marked method builds string by the format pattern and (optional) arguments.
	///     The parameter, which contains the format string, should be given in constructor. The format string
	///     should be in <see cref="String.Format(IFormatProvider,String,System.Object[])" />-like form.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Constructor
		| AttributeTargets.Method
		| AttributeTargets.Property
		| AttributeTargets.Delegate
	)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		public String FormatParameterName { get; }

		/// <inheritdoc />
		/// <param name="formatParameterName">
		///     Specifies which parameter of an annotated method should be treated as the format string
		/// </param>
		public StringFormatMethodAttribute (String formatParameterName)
		{
			FormatParameterName = formatParameterName;
		}
	}
}
