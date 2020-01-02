// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.DataContracts;
	using Arcanum.Routes;
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;

	public sealed class UnionJsonContractFactory: IJsonContractFactory {
		/// <summary>
		///     Writes to <paramref name = "output" /> case json. Returns case type.
		/// </summary>
		static Type ExtractCaseType (JsonReader input, JsonWriter output, IUnionInfo unionInfo) {
			static IUnionCaseInfo ReadCaseInfo (JsonReader reader, IUnionInfo unionInfo) {
				if (reader.TokenType != JsonToken.String)
					throw reader.Exception("Expected '$case' to be string but accepted {0}", reader.TokenType);

				var caseRouteStr = (String) reader.Value!;
				var caseRoute = (Route) caseRouteStr;
				try {
					return unionInfo.GetNestedCaseInfo(caseRoute);
				}
				catch (FormatException ex) {
					var msg = $"Failed to find case '{caseRoute}' in '{unionInfo}'.";
					throw new JsonSerializationException(msg, ex);
				}
			}

			input.CurrentTokenMustBe(JsonToken.StartObject);
			input.ReadNext();

			Type? unionCaseType = null;
			Boolean? hasValueProp = null;

			while (input.TokenType != JsonToken.EndObject) {
				input.CurrentTokenMustBe(JsonToken.PropertyName);
				var curPropNameString = (String) input.Value!;

				switch (curPropNameString) {
					case "$case" when unionCaseType is { }:
						throw
							input.Exception("The property '$case' is contained twice in the object.");
					case "$case":
						input.ReadNext();
						unionCaseType = ReadCaseInfo(input, unionInfo).dataType;
						break;
					case "$values":
					case "$value":
						switch (hasValueProp) {
							case false:
								throw
									input.Exception(
										"Cannot deserialize object with '{0}' and its own properties at the same time.",
										curPropNameString);
							case true:
								throw
									input.Exception(
										"The property '{0}' is contained twice in the object.",
										curPropNameString);
							case null:
								hasValueProp = true;
								input.ReadNext();
								output.WriteToken(input, writeChildren: true);
								break;
						}
						break;
					default:
						switch (hasValueProp) {
							case true:
								throw
									input.Exception(
										"Cannot deserialize object with '$values' or '$value' and its own properties"
										+ " at the same time.");
							case null:
								hasValueProp = false;
								output.WriteStartObject();
								goto case false;
							case false:
								output.WriteToken(input);
								input.ReadNext();
								output.WriteToken(input, writeChildren: true);
								break;
						}
						break;
				}

				input.ReadNext();
			}
			switch (hasValueProp) {
				case null:
					output.WriteStartObject();
					output.WriteEndObject();
					break;
				case false:
					output.WriteEndObject();
					break;
			}

			return
				unionCaseType
				?? throw
					input.Exception(
						"The property '$case' is not found. It's required to deserialize discriminated union {0}"
						+ " to a specific case.",
						unionInfo.dataType);
		}

		class UnionJsonConverter: JsonConverterAdapter, IReadJsonConverter {
			IUnionInfo unionInfo { get; }

			public UnionJsonConverter (IUnionInfo unionInfo) =>
				this.unionInfo = unionInfo;

			/// <inheritdoc />
			public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) {
				if (reader.TokenType is JsonToken.Null) return null;

				using (var source = JsonMemory.Rent()) {
					Type unionCaseType; // I guess 'unionCaseType' is always assignable to 'objectType'.
					using (var writer = source.Write())
						unionCaseType = ExtractCaseType(input: reader, output: writer, unionInfo);

					using (var sourceReader = source.Read())
						return serializer.Deserialize(sourceReader, unionCaseType);
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
