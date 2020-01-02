// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Immutable;

	public sealed class EnumJsonContractFactory: IJsonContractFactory {
		class EnumJsonConverter: JsonConverterAdapter, IToJsonConverter, IFromJsonConverter {
			Type dataType { get; }

			ImmutableDictionary<String, Object> values { get; }

			public EnumJsonConverter (Type dataType) {
				this.dataType = dataType;

				var names = dataType.GetEnumNames();
				var mutValues = ImmutableDictionary.CreateBuilder<String, Object>();
				var pos = 0;
				foreach (var value in dataType.GetEnumValues()) {
					mutValues.Add(names[pos], value);
					pos = checked(pos + 1);
				}
				values = mutValues.ToImmutable();
			}

			/// <inheritdoc />
			public void Write (JsonWriter writer, Object value, JsonSerializer serializer) {
				var valueName = dataType.GetEnumName(value);
				writer.WriteValue(valueName);
			}

			String ReadEnumName (JsonReader reader) {
				switch (reader.TokenType) {
					case JsonToken.String:
						return (String) reader.Value!;
					case JsonToken.StartObject: {
						reader.ReadNext();
						while (reader.TokenType != JsonToken.EndObject) {
							reader.CurrentTokenMustBe(JsonToken.PropertyName);
							var propName = (String) reader.Value!;
							reader.ReadNext();
							if (propName is "$case") {
								reader.CurrentTokenMustBe(JsonToken.String);
								return (String) reader.Value!;
							}
							reader.ReadNext();
						}
						var msg = "Failed to read enum '{0}': object doesn't contain property '$case'.";
						throw reader.Exception(msg, dataType);
					}
					case var tokenType: {
						var msg = "Failed to read enum '{0}': Expected token to be string or object but accepted {1}.";
						throw reader.Exception(msg, dataType, tokenType);
					}
				}
			}

			/// <inheritdoc />
			public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) {
				var valueName = ReadEnumName(reader);
				if (values.TryGetValue(valueName, out var value))
					return value;
				else {
					var msg = "Failed to read enum '{0}': '{1}' is not a value.";
					throw reader.Exception(msg, dataType, valueName);
				}
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			if (request.dataType.IsEnum && ! Attribute.IsDefined(request.dataType, typeof(FlagsAttribute))) {
				var converter = new EnumJsonConverter(request.dataType);
				var contract = new JsonLinqContract(request.dataType) { Converter = converter };
				request.Return(contract);
			}
		}
	}
}
