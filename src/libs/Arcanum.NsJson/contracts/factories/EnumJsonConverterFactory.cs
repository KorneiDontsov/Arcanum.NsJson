// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using System.Collections.Immutable;

	public sealed class EnumJsonConverterFactory: IJsonConverterFactory {
		class EnumJsonConverter: IJsonConverter {
			Type dataType { get; }

			ImmutableDictionary<String, Object> values { get; }

			public EnumJsonConverter (Type dataType) {
				this.dataType = dataType;

				var names = dataType.GetEnumNames();
				var mutValues = ImmutableDictionary.CreateBuilder<String, Object>();
				var pos = 0;
				foreach(var value in dataType.GetEnumValues()) {
					mutValues.Add(names[pos], value);
					pos = checked(pos + 1);
				}
				values = mutValues.ToImmutable();
			}

			/// <inheritdoc />
			public void Write (IJsonSerializer serializer, JsonWriter writer, Object value, ILocalsCollection locals) {
				var valueName = dataType.GetEnumName(value);
				writer.WriteValue(valueName);
			}

			String ReadEnumName (JsonReader reader) {
				switch(reader.TokenType) {
					case JsonToken.String:
						return (String) reader.Value!;
					case JsonToken.StartObject: {
						reader.ReadNext();
						while(reader.TokenType != JsonToken.EndObject) {
							reader.CurrentTokenMustBe(JsonToken.PropertyName);
							var propName = (String) reader.Value!;
							reader.ReadNext();
							if(propName is "$case") {
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
			public Object Read (IJsonSerializer serializer, JsonReader reader, ILocalsCollection locals) {
				var valueName = ReadEnumName(reader);
				if(values.TryGetValue(valueName, out var value))
					return value;
				else {
					var msg = "Failed to read enum '{0}': '{1}' is not a value.";
					throw reader.Exception(msg, dataType, valueName);
				}
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonConverterRequest request) {
			if(request.dataType.IsEnum && ! Attribute.IsDefined(request.dataType, typeof(FlagsAttribute)))
				request.Return(new EnumJsonConverter(request.dataType));
		}
	}
}
