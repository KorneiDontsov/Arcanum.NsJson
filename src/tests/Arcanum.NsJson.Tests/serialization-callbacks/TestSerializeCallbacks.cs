// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tests {
	using Arcanum.NsJson.Contracts;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using System;
	using System.Runtime.Serialization;
	using Xunit;

	public class TestSerializeCallbacks: TestJsonSerializer {
		abstract class DataBase {
			public Boolean baseWhenSerializing { get; private set; }

			public Boolean baseWhenSerialized { get; private set; }

			[OnSerializing, UsedImplicitly]
			void OnSerializing (StreamingContext context) =>
				baseWhenSerializing = true;

			[OnSerialized, UsedImplicitly]
			void OnSerialized (StreamingContext context) =>
				baseWhenSerialized = true;
		}

		sealed class Data: DataBase {
			class ToJsonConverter: IToJsonConverter {
				/// <inheritdoc />
				public void Write
					(IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) =>
					writer.WriteValue("data-str");
			}

			[UsedImplicitly]
			public class Companion: IJsonConverterFactory {
				/// <inheritdoc />
				public void Handle (IJsonConverterRequest request) =>
					request.ReturnWriteOnly(new ToJsonConverter());
			}

			public Boolean whenSerializing { get; private set; }

			public Boolean whenSerialized { get; private set; }

			public Boolean whenSerializingAfterBase { get; private set; }

			public Boolean whenSerializedAfterBase { get; private set; }

			public Boolean whenSerializedAfterSerializing { get; private set; }

			[OnSerializing, UsedImplicitly]
			void OnSerializing (StreamingContext context) {
				whenSerializing = true;
				whenSerializingAfterBase = baseWhenSerializing;
			}

			[OnSerialized, UsedImplicitly]
			void OnSerialized (StreamingContext context) {
				whenSerialized = true;
				whenSerializedAfterBase = baseWhenSerialized;
				whenSerializedAfterSerializing = whenSerializing;
			}
		}

		Data data { get; }

		public TestSerializeCallbacks () {
			data = new Data();
			_ = serializer.ToToken(data);
		}

		[Fact]
		public void OnSerializingCallbackIsCalled () =>
			data.whenSerializing.Should().BeTrue();

		[Fact]
		public void OnSerializingCallbackOfBaseTypeIsCalled () =>
			data.baseWhenSerializing.Should().BeTrue();

		[Fact]
		public void OnSerializedCallbackIsCalled () =>
			data.whenSerialized.Should().BeTrue();

		[Fact]
		public void OnSerializedCallbackOfBaseTypeIsCalled () =>
			data.baseWhenSerialized.Should().BeTrue();

		[Fact]
		public void OnSerializingCallbackOfBaseTypeIsCalledBeforeOnSerializingCallbackOfDerivedType () =>
			data.whenSerializingAfterBase.Should().BeTrue();

		[Fact]
		public void OnSerializedCallbackOfBaseTypeIsCalledBeforeOnSerializedCallbackOfDerivedType () =>
			data.whenSerializedAfterBase.Should().BeTrue();

		[Fact]
		public void OnSerializedCallbackIsCalledAfterOnSerializingCallback () =>
			data.whenSerializedAfterSerializing.Should().BeTrue();
	}
}
