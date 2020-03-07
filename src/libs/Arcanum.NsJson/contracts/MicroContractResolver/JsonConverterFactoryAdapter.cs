// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;

	class JsonConverterFactoryAdapter: IJsonContractFactory {
		class JsonConverterRequest: IJsonConverterRequest {
			Boolean returned;

			IToJsonConverter? toJsonConverter;

			IFromJsonConverter? fromJsonConverter;

			/// <inheritdoc />
			public Type dataType { get; }

			public JsonConverterRequest (Type dataType) =>
				this.dataType = dataType;

			public (Boolean returned, IToJsonConverter? toJsonConverter, IFromJsonConverter? fromJsonConverter)
				Request (IJsonConverterFactory jsonConverterFactory) {
				try {
					jsonConverterFactory.Handle(this);
					return (returned, toJsonConverter, fromJsonConverter);
				}
				finally {
					returned = false;
					toJsonConverter = null;
					fromJsonConverter = null;
				}
			}

			void SetReturned () {
				if (returned)
					throw new Exception("Return method invoked twice.");
				else
					returned = true;
			}

			/// <inheritdoc />
			public void Return (IJsonConverter jsonConverter) {
				SetReturned();
				toJsonConverter = jsonConverter;
				fromJsonConverter = jsonConverter;
			}

			/// <inheritdoc />
			public void ReturnReadOnly (IFromJsonConverter fromJsonConverter) {
				SetReturned();
				this.fromJsonConverter = fromJsonConverter;
			}

			/// <inheritdoc />
			public void ReturnWriteOnly (IToJsonConverter toJsonConverter) {
				SetReturned();
				this.toJsonConverter = toJsonConverter;
			}
		}

		class JsonConverterAdapter: JsonConverter {
			Type dataType { get; }
			IToJsonConverter? toJsonConverter { get; }
			IFromJsonConverter? fromJsonConverter { get; }

			public JsonConverterAdapter
				(Type dataType, IToJsonConverter? toJsonConverter, IFromJsonConverter? fromJsonConverter) {
				this.dataType = dataType;
				this.toJsonConverter = toJsonConverter;
				this.fromJsonConverter = fromJsonConverter;
			}

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) => true;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) {
				if (value is null)
					writer.WriteNull();
				else if (toJsonConverter is null)
					throw new JsonSerializationException($"Type {dataType} is not serializable to json.");
				else {
					var arcaneSerializer = serializer.Arcane();
					using var context = arcaneSerializer.CaptureContext();
					toJsonConverter.Write(arcaneSerializer, writer, value, context.locals);
				}
			}

			/// <inheritdoc />
			public override Object? ReadJson
				(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) {
				if (reader.TokenType is JsonToken.Null)
					return null;
				else if (fromJsonConverter is null)
					throw new JsonSerializationException($"Type {dataType} is not serializable from json.");
				else {
					var arcaneSerializer = serializer.Arcane();
					using var context = arcaneSerializer.CaptureContext();
					return fromJsonConverter.Read(arcaneSerializer, reader, context.locals);
				}
			}
		}

		IJsonConverterFactory converterFactory { get; }

		public JsonConverterFactoryAdapter (IJsonConverterFactory converterFactory) =>
			this.converterFactory = converterFactory;

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			var converterRequest = new JsonConverterRequest(request.dataType);
			var (returned, toJsonConverter, fromJsonConverter) = converterRequest.Request(converterFactory);
			if (returned) {
				var converterAdapter = new JsonConverterAdapter(request.dataType, toJsonConverter, fromJsonConverter);
				request.Return(new JsonLinqContract(request.dataType) { Converter = converterAdapter });
			}
		}
	}
}
