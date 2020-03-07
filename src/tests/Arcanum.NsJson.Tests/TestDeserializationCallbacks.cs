// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Runtime.Serialization;
	using Xunit;

	public class TestDeserializationCallbacks: TestJsonSerializer {
		abstract class DataBase {
			public Boolean baseWhenDeserializing { get; private set; }

			public Boolean baseWhenDeserialized { get; private set; }

			[OnDeserializing, UsedImplicitly]
			void OnDeserializing (StreamingContext context) =>
				baseWhenDeserializing = true;

			[OnDeserialized, UsedImplicitly]
			void OnDeserialized (StreamingContext context) =>
				baseWhenDeserialized = true;
		}

		sealed class Data: DataBase {
			class FromJsonConverter: IFromJsonConverter {
				/// <inheritdoc />
				public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
					reader.ReadAsString();
					return new Data();
				}
			}

			[UsedImplicitly]
			public class Companion: IJsonConverterFactory {
				/// <inheritdoc />
				public void Handle (IJsonConverterRequest request) =>
					request.ReturnReadOnly(new FromJsonConverter());
			}

			public Boolean whenDeserializing { get; private set; }

			public Boolean whenDeserialized { get; private set; }

			public Boolean whenDeserializedAfterBase { get; private set; }

			[OnDeserializing, UsedImplicitly]
			void OnDeserializing (StreamingContext context) =>
				whenDeserializing = true;

			[OnDeserialized, UsedImplicitly]
			void OnDeserialized (StreamingContext context) {
				whenDeserialized = true;
				whenDeserializedAfterBase = true;
			}
		}

		Data data { get; }

		public TestDeserializationCallbacks () {
			var token = new JValue("any-string");
			data = serializer.FromToken<Data>(token);
		}

		[Fact]
		public void OnDeserializingCallbacksAreNotCalled () {
			data.baseWhenDeserializing.Should().BeFalse();
			data.whenDeserializing.Should().BeFalse();
		}

		[Fact]
		public void DefaultOnDeserializedCallbackIsCalled () =>
			data.whenDeserialized.Should().BeTrue();

		[Fact]
		public void DefaultOnDeserializedCallbackOfBaseTypeIsCalled () =>
			data.baseWhenDeserialized.Should().BeTrue();

		[Fact]
		public void CallbacksOfBaseTypeAreCalledBeforeCallbacksOfDerivedType () =>
			data.whenDeserializedAfterBase.Should().BeTrue();
	}
}
