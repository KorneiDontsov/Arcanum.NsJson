// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;

	public sealed class NsJsonBasedContractFactory: IJsonContractFactory {
		sealed class JsonPropertyTopLevelConverter: JsonConverter {
			readonly String propertyName;
			readonly JsonConverter? baseConverter;

			public JsonPropertyTopLevelConverter (String propertyName, JsonConverter? baseConverter) {
				this.propertyName = propertyName;
				this.baseConverter = baseConverter;
			}

			public override Boolean CanConvert (Type objectType) => true;

			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) {
				if(baseConverter is {CanWrite: true}
				   && (value is null || baseConverter.CanConvert(value.GetType())))
					baseConverter.WriteJson(writer, value, serializer);
				else
					serializer.Serialize(writer, value);
			}

			public override Object? ReadJson
				(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) {
				using var localsOwner = serializer.Arcane().CaptureLocals();
				var samples = localsOwner.locals.MaybeSamples();

				// Set member samples.
				if(samples?.createMemberSamples.GetValueOrDefault(propertyName) is {} createMemberSample)
					localsOwner.locals.SetSamples(new LocalSamples(objectType, createSelfSample: createMemberSample));

				if(baseConverter is {CanRead: true} && baseConverter.CanConvert(objectType))
					return baseConverter.ReadJson(reader, objectType, existingValue, serializer);
				else
					return serializer.Deserialize(reader, objectType);
			}
		}

		class Resolver: DefaultContractResolver {
			public Resolver () =>
				SerializeCompilerGeneratedMembers = true;

			protected override IList<JsonProperty> CreateProperties
				(Type type, MemberSerialization memberSerialization) {
				var properties = base.CreateProperties(type, memberSerialization);
				foreach(var property in properties)
					if(property.PropertyName is {} propertyName)
						property.Converter = new JsonPropertyTopLevelConverter(propertyName, property.Converter);
				return properties;
			}

			public new JsonContract CreateContract (Type dataType) {
				var contract = base.CreateContract(dataType);
				contract.OnDeserializingCallbacks.Clear();
				contract.OnDeserializedCallbacks.Clear();
				contract.OnErrorCallbacks.Clear();
				contract.IsReference = false;
				return contract;
			}
		}

		static Resolver resolver { get; } = new Resolver();

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			var contract = resolver.CreateContract(request.dataType);
			request.Return(contract);
		}
	}
}
