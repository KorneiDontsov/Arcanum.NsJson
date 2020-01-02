// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using Arcanum.Routes;
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;

	public sealed class UnionJsonContractFactory: IJsonContractFactory {
		class UnionJsonConverter: JsonConverterAdapter, IReadJsonConverter {
			IUnionInfo unionInfo { get; }

			public UnionJsonConverter (IUnionInfo unionInfo) =>
				this.unionInfo = unionInfo;

			IUnionCaseInfo ReadCaseInfo (JsonReader reader) {
				if (reader.TokenType != JsonToken.String) {
					var msg = "Expected string with union case route but accepted {0}";
					throw reader.Exception(msg, reader.TokenType);
				}
				else {
					var caseRoute = (Route) (String) reader.Value!;
					try {
						return unionInfo.GetNestedCaseInfo(caseRoute);
					}
					catch (FormatException ex) {
						var msg = $"Failed to find case '{caseRoute}' in '{unionInfo}'.";
						throw new JsonSerializationException(msg, ex);
					}
				}
			}

			IUnionCaseInfo ExtractCaseInfoFromString (JsonReader reader, JsonWriter writer) {
				var unionCaseInfo = ReadCaseInfo(reader);

				writer.WriteStartObject();
				writer.WriteEndObject();

				return unionCaseInfo;
			}

			enum TargetCase { Unknown, Object, Value }

			IUnionCaseInfo ExtractCaseInfoFromObject (JsonReader reader, JsonWriter writer) {
				reader.CurrentTokenMustBe(JsonToken.StartObject);
				reader.ReadNext();

				IUnionCaseInfo? unionCaseInfo = null;
				var targetCase = TargetCase.Unknown;

				while (reader.TokenType != JsonToken.EndObject) {
					reader.CurrentTokenMustBe(JsonToken.PropertyName);
					var curPropName = (String) reader.Value!;

					switch (curPropName) {
						case "$case" when unionCaseInfo is { }:
							throw reader.Exception("The property '$case' is contained twice in the object.");
						case "$case":
							reader.ReadNext();
							unionCaseInfo = ReadCaseInfo(reader);
							break;
						case "$values" when targetCase == TargetCase.Unknown:
						case "$value" when targetCase == TargetCase.Unknown:
							targetCase = TargetCase.Value;
							reader.ReadNext();
							writer.WriteToken(reader, writeChildren: true);
							break;
						case var _ when targetCase != TargetCase.Value:
							if (targetCase == TargetCase.Unknown) {
								targetCase = TargetCase.Object;
								writer.WriteStartObject();
							}

							writer.WritePropertyName(curPropName);
							reader.ReadNext();
							writer.WriteToken(reader, writeChildren: true);

							break;
					}

					reader.ReadNext();
				}

				if (unionCaseInfo is null) {
					var msg =
						"The property '$case' is not found. It's required to deserialize '{0}' to a specific case.";
					throw reader.Exception(msg, unionInfo);
				}
				else {
					switch (targetCase) {
						case TargetCase.Unknown:
							writer.WriteStartObject();
							writer.WriteEndObject();
							break;
						case TargetCase.Object:
							writer.WriteEndObject();
							break;
					}

					return unionCaseInfo;
				}
			}

			/// <inheritdoc />
			public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) {
				static Exception BadTokenException (JsonReader reader) =>
					reader.Exception("Expected token to be string or object but accepted {0}", reader.TokenType);

				if (reader.TokenType is JsonToken.Null)
					return null;
				else {
					using var source = JsonMemory.Rent();

					IUnionCaseInfo unionCaseInfo;
					using (var writer = source.Write())
						unionCaseInfo =
							reader.TokenType switch {
								JsonToken.String => ExtractCaseInfoFromString(reader, writer),
								JsonToken.StartObject => ExtractCaseInfoFromObject(reader, writer),
								_ => throw BadTokenException(reader)
							};

					using (var targetReader = source.Read())
						return serializer.Deserialize(targetReader, unionCaseInfo.dataType);
				}
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonContractRequest request) {
			var matched = request.dataType.IsClass && request.dataType.IsAbstract;
			if (matched && GetDataTypeInfo(request.dataType).asUnionInfo is {} unionInfo) {
				if (unionInfo.hasErrors) {
					var message =
						$"Cannot resolve {unionInfo.dataType} because it has errors.\n{unionInfo.GetErrorString()}";
					throw new JsonContractException(message);
				}
				else {
					var contract = BasicJsonContractFactory.CreateContract(request.dataType);
					contract.Converter ??= new UnionJsonConverter(unionInfo.rootUnionInfo);
					request.Return(contract);
				}
			}
		}
	}
}
