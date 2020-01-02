// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using FluentAssertions.Json;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Linq;
	using System.Reflection;
	using Xunit;

	public class TestUseJsonContractFactory: TestJsonSerializer {
		interface IDefaultValueProvider {
			Object? defaultValue { get; }
		}

		[OptionalJsonContractFactory]
		class Optional<T> where T: struct {
			public T maybeValue { get; set; }
		}

		class OptionalJsonContractFactory: Attribute, IJsonContractFactory {
			/// <inheritdoc />
			public void Handle (IJsonContractRequest request) {
				var valueType = request.dataType.GetGenericArguments()[0];
				var valueDefault =
					valueType.GetCustomAttributes()
						.Select(a => a as IDefaultValueProvider).FirstOrDefault(a => a is {})
						?.defaultValue;
				var valueProp =
					new JsonProperty {
						PropertyName = "Value",
						UnderlyingName = "maybeValue",
						PropertyType = valueType,
						DeclaringType = request.dataType,
						Readable = true,
						Writable = true,
						ValueProvider = new ReflectionValueProvider(request.dataType.GetProperty("maybeValue")!),
						Required = Required.Default,
						DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
						DefaultValue = valueDefault
					};
				var contract =
					new JsonObjectContract(request.dataType) {
						MemberSerialization = MemberSerialization.OptOut,
						Properties = { valueProp },
						DefaultCreator = () => Activator.CreateInstance(request.dataType)!
					};
				request.Return(contract);
			}
		}

		[DefaultValueProvider]
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		readonly struct HttpPort: IEquatable<HttpPort> {
			public UInt16 value { get; }

			public HttpPort (UInt16 value) => this.value = value;

			/// <inheritdoc />
			public Boolean Equals (HttpPort other) =>
				value == other.value;

			/// <inheritdoc />
			public override Boolean Equals (Object? obj) =>
				obj is HttpPort other && Equals(other);

			/// <inheritdoc />
			public override Int32 GetHashCode () => value.GetHashCode();

			class DefaultValueProvider: Attribute, IDefaultValueProvider {
				/// <inheritdoc />
				public Object? defaultValue { get; } = new HttpPort(80);
			}
		}

		[Fact]
		public void TargetIsSerialized () {
			var data = new Optional<HttpPort> { maybeValue = new HttpPort(433) };
			var expected = new JObject { ["Value"] = new JObject { ["value"] = 433 } };
			var actual = serializer.ToToken(data);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void TargetIsDeserialized () {
			var token = new JObject { ["Value"] = new JObject { ["value"] = 433 } };
			var expectedPort = new HttpPort(433);
			var actualPortOptional = serializer.FromToken<Optional<HttpPort>>(token);
			actualPortOptional.maybeValue.Should().Be(expectedPort);
		}

		[Fact]
		public void TargetIsSerializedWithDefaultValue () {
			var data = new Optional<HttpPort> { maybeValue = new HttpPort(80) };
			var expected = new JObject();
			var actual = serializer.ToToken(data);
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void TargetIsDeserializedWithDefaultValue () {
			var token = new JObject();
			var expectedPort = new HttpPort(80);
			var actualPortOptional = serializer.FromToken<Optional<HttpPort>>(token);
			actualPortOptional.maybeValue.Should().Be(expectedPort);
		}
	}
}
