// https://github.com/dotnet/corefx/blob/48363ac826ccf66fbe31a5dcb1dc2aab9a7dd768/src/Common/src/CoreLib/System/Diagnostics/CodeAnalysis/NullableAttributes.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable InconsistentNaming

namespace System.Diagnostics.CodeAnalysis {
	/// <inheritdoc />
	/// <summary> Specifies that null is allowed as an input even if the corresponding type disallows it. </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	sealed class AllowNullAttribute: Attribute { }

	/// <inheritdoc />
	/// <summary> Specifies that null is disallowed as an input even if the corresponding type allows it. </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	sealed class DisallowNullAttribute: Attribute { }

	/// <inheritdoc />
	/// <summary> Specifies that an output may be null even if the corresponding type disallows it. </summary>
	[AttributeUsage(
		AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	sealed class MaybeNullAttribute: Attribute { }

	/// <inheritdoc />
	/// <summary> Specifies that an output will not be null even if the corresponding type allows it. </summary>
	[AttributeUsage(
		AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	sealed class NotNullAttribute: Attribute { }

	/// <inheritdoc />
	/// <summary>
	///     Specifies that when a method returns
	///     <see cref = "P:System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute.ReturnValue" />, the
	///     parameter may be null even if the corresponding type disallows it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class MaybeNullWhenAttribute: Attribute {
		/// <inheritdoc />
		/// <summary> Initializes the attribute with the specified return value condition. </summary>
		/// <param name = "returnValue">
		///     The return value condition. If the method returns this value, the associated parameter may be
		///     null.
		/// </param>
		public MaybeNullWhenAttribute (Boolean returnValue) => ReturnValue = returnValue;

		/// <summary> Gets the return value condition. </summary>
		public Boolean ReturnValue { get; }
	}

	/// <inheritdoc />
	/// <summary>
	///     Specifies that when a method returns
	///     <see cref = "P:System.Diagnostics.CodeAnalysis.NotNullWhenAttribute.ReturnValue" />, the
	///     parameter will not
	///     be null even if the corresponding type allows it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class NotNullWhenAttribute: Attribute {
		/// <inheritdoc />
		/// <summary> Initializes the attribute with the specified return value condition. </summary>
		/// <param name = "returnValue">
		///     The return value condition. If the method returns this value, the associated parameter will not
		///     be null.
		/// </param>
		public NotNullWhenAttribute (Boolean returnValue) => ReturnValue = returnValue;

		/// <summary> Gets the return value condition. </summary>
		public Boolean ReturnValue { get; }
	}

	/// <inheritdoc />
	/// <summary> Specifies that the output will be non-null if the named parameter is non-null. </summary>
	[AttributeUsage(
		AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
	sealed class NotNullIfNotNullAttribute: Attribute {
		/// <inheritdoc />
		/// <summary> Initializes the attribute with the associated parameter name. </summary>
		/// <param name = "parameterName">
		///     The associated parameter name.  The output will be non-null if the argument to the parameter
		///     specified is non-null.
		/// </param>
		public NotNullIfNotNullAttribute (String parameterName) => ParameterName = parameterName;

		/// <summary> Gets the associated parameter name. </summary>
		public String ParameterName { get; }
	}

	/// <inheritdoc />
	/// <summary> Applied to a method that will never return under any circumstance. </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	sealed class DoesNotReturnAttribute: Attribute { }

	/// <inheritdoc />
	/// <summary>
	///     Specifies that the method will not return if the associated Boolean parameter is passed
	///     the specified value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class DoesNotReturnIfAttribute: Attribute {
		/// <inheritdoc />
		/// <summary> Initializes the attribute with the specified parameter value. </summary>
		/// <param name="parameterValue">
		///     The condition parameter value. Code after the method will be considered unreachable by
		///     diagnostics if the argument to
		///     the associated parameter matches this value.
		/// </param>
		public DoesNotReturnIfAttribute (Boolean parameterValue) => ParameterValue = parameterValue;

		/// <summary> Gets the condition parameter value. </summary>
		public Boolean ParameterValue { get; }
	}
}
