// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using FluentAssertions;
	using FluentAssertions.Json;
	using Newtonsoft.Json.Linq;
	using Xunit;

	public class TestEnums: TestJsonSerializer {
		public enum EnumExample {
			Case1,
			Case2,
			Case3
		}

		[Theory]
		[InlineData(EnumExample.Case1)]
		[InlineData(EnumExample.Case2)]
		[InlineData(EnumExample.Case3)]
		public void IsSerializedToJsonString (EnumExample enumCase) {
			var expectedToken = new JValue(enumCase.ToString());
			var actualToken = serializer.ToToken(enumCase);
			actualToken.Should().BeEquivalentTo(expectedToken);
		}

		[Theory]
		[InlineData(EnumExample.Case1)]
		[InlineData(EnumExample.Case2)]
		[InlineData(EnumExample.Case3)]
		public void IsDeserializedFromJsonString (EnumExample enumCase) {
			var token = new JValue(enumCase.ToString());
			var actualEnumCase = serializer.FromToken<EnumExample>(token);
			actualEnumCase.Should().Be(enumCase);
		}
	}
}
