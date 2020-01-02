﻿// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Threading;

	class MicroContractResolver: IContractResolver {
		ImmutableDictionary<Type, IJsonContractCreator> contractCreators { get; }
		ImmutableArray<IJsonContractFactory> contractFactories { get; }
		ImmutableArray<IJsonContractPatch> contractPatches { get; }
		ImmutableArray<IJsonMiddlewareFactory> middlewarePatches { get; }
		JsonContractStorage contractStorage { get; }
		JsonContractStorage middlewareContractStorage { get; }

		class JsonContractRequest: IJsonContractRequest {
			/// <inheritdoc />
			public Type dataType { get; }

			JsonContract? contract;

			public JsonContractRequest (Type dataType) => this.dataType = dataType;

			/// <inheritdoc />
			public void Return (JsonContract contract) {
				if (this.contract is null)
					this.contract = contract;
				else
					throw new Exception("Cannot return contract second time.");
			}

			public JsonContract? RequestContract (ImmutableArray<IJsonContractFactory> contractFactories) {
				foreach (var contractFactory in contractFactories) {
					contractFactory.Handle(this);
					if (contract is {}) return contract;
				}
				return null;
			}
		}

		/// <summary>
		///     <paramref name = "dataType" /> is not ByRef type because execution go through
		///     <see cref = "CreateMiddlewareContract" />.
		/// </summary>
		/// <exception cref = "JsonContractException" />
		JsonContract CreateContract (Type dataType) {
			if (dataType.MatchCustomAttribute<IJsonContractCreator>(inherit: false) is {} externalCreator)
				return externalCreator.CreateContract();
			else {
				var contractRequest = new JsonContractRequest(dataType);
				var externalFactories = dataType.MatchCustomAttributes<IJsonContractFactory>();
				var contract =
					contractRequest.RequestContract(externalFactories)
					?? contractCreators.GetValueOrDefault(dataType)?.CreateContract()
					?? contractRequest.RequestContract(contractFactories)
					?? throw new JsonContractException($"{dataType} has no contract.");
				foreach (var contractPatch in contractPatches) contractPatch.Patch(contract);
				return contract;
			}
		}

		static AsyncLocal<Boolean> noMiddleware { get; } = new AsyncLocal<Boolean>();

		class MiddlewareJsonConverter: JsonConverterAdapter, IWriteJsonConverter, IReadJsonConverter {
			WriteJson writeJson { get; }
			ReadJson readJson { get; }

			public MiddlewareJsonConverter (WriteJson writeJson, ReadJson readJson) {
				this.writeJson = writeJson;
				this.readJson = readJson;
			}

			/// <inheritdoc />
			public void Write (JsonWriter writer, Object value, JsonSerializer serializer) =>
				writeJson(writer, value, serializer);

			/// <inheritdoc />
			public Object? Read (JsonReader reader, Type dataType, JsonSerializer serializer) =>
				readJson(reader, serializer);
		}

		class JsonMiddlewareRequest: IJsonMiddlewareRequest {
			List<IJsonReadMiddleware> readMiddlewares { get; } = new List<IJsonReadMiddleware>();

			List<IJsonWriteMiddleware> writeMiddlewares { get; } = new List<IJsonWriteMiddleware>();

			/// <inheritdoc />
			public Type dataType { get; }

			public JsonMiddlewareRequest (Type dataType) => this.dataType = dataType;

			/// <inheritdoc />
			public void Yield (IJsonWriteMiddleware writeMiddleware) =>
				writeMiddlewares.Add(writeMiddleware);

			/// <inheritdoc />
			public void Yield (IJsonReadMiddleware readMiddleware) =>
				readMiddlewares.Add(readMiddleware);

			WriteJson BuildWrite () {
				WriteJson write =
					(writer, value, serializer) => {
						using (new AsyncLocalTrigger(noMiddleware))
							serializer.Serialize(writer, value, dataType);
					};
				for (var i = writeMiddlewares.Count - 1; i >= 0; i -= 1) {
					var middleware = writeMiddlewares[i];
					var previous = write;
					write = (writer, value, serializer) => middleware.WriteJson(writer, value, serializer, previous);
				}

				return write;
			}

			ReadJson BuildRead () {
				ReadJson read =
					(reader, serializer) => {
						using (new AsyncLocalTrigger(noMiddleware))
							return serializer.Deserialize(reader, dataType);
					};
				foreach (var middleware in readMiddlewares) {
					var next = read;
					read = (reader, serializer) => middleware.ReadJson(reader, serializer, next);
				}

				return read;
			}

			public JsonConverter? MayBuildConverter () {
				if (writeMiddlewares.Count > 0 || readMiddlewares.Count > 0) {
					var write = BuildWrite();
					var read = BuildRead();
					return new MiddlewareJsonConverter(write, read);
				}
				else
					return null;
			}
		}

		/// <exception cref = "JsonContractException" />
		JsonContract CreateMiddlewareContract (Type dataType) {
			if (dataType.IsByRef && dataType.HasElementType)
				return middlewareContractStorage.GetOrCreate(dataType.GetElementType());
			else {
				var contract = contractStorage.GetOrCreate(dataType);
				var middlewareRequest = new JsonMiddlewareRequest(dataType);

				foreach (var middlewarePatch in middlewarePatches)
					middlewarePatch.Handle(middlewareRequest);

				return middlewareRequest.MayBuildConverter() is {} middlewareConverter
					? contract.Copy().AddConverter(middlewareConverter)
					: contract;
			}
		}

		public MicroContractResolver
		(ImmutableDictionary<Type, IJsonContractCreator> contractCreators,
		 ImmutableArray<IJsonContractFactory> contractFactories,
		 ImmutableArray<IJsonContractPatch> contractPatches,
		 ImmutableArray<IJsonMiddlewareFactory> middlewarePatches) {
			this.contractCreators = contractCreators;
			this.contractFactories = contractFactories;
			this.contractPatches = contractPatches;
			this.middlewarePatches = middlewarePatches;
			contractStorage = new JsonContractStorage(CreateContract);
			middlewareContractStorage = new JsonContractStorage(CreateMiddlewareContract);
		}

		/// <inheritdoc />
		/// <exception cref = "JsonContractException" />
		public JsonContract ResolveContract (Type dataType) =>
			noMiddleware.Value
				? contractStorage.GetOrCreate(dataType)
				: middlewareContractStorage.GetOrCreate(dataType);
	}
}
