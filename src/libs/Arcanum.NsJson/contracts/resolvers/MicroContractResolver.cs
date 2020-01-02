// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

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
		ImmutableArray<IJsonMiddlewarePatch> middlewarePatches { get; }
		JsonContractStorage contractStorage { get; }
		JsonContractStorage middlewareContractStorage { get; }

		/// <summary>
		///     <paramref name = "dataType" /> is not ByRef type because execution go through
		///     <see cref = "CreateMiddlewareContract" />.
		/// </summary>
		/// <exception cref = "JsonContractException" />
		JsonContract CreateContract (Type dataType) {
			static JsonContract? MayCreateContract
			(ImmutableArray<IJsonContractFactory> contractFactories,
			 Type dataType) {
				foreach (var contractFactory in contractFactories)
					if (contractFactory.MayCreateContract(dataType) is {} contract)
						return contract;
				return null;
			}

			var contract =
				contractCreators.GetValueOrDefault(dataType)?.CreateContract()
				?? MayCreateContract(contractFactories, dataType)
				?? throw new JsonContractException($"{dataType} has no contract.");

			foreach (var contractPatch in contractPatches) contractPatch.Patch(contract);

			return contract;
		}

		static AsyncLocal<Boolean> noMiddleware { get; } = new AsyncLocal<Boolean>();

		class JsonMiddlewareBuilder: IJsonMiddlewareBuilder {
			List<IJsonReadMiddleware> readMiddlewares { get; } = new List<IJsonReadMiddleware>();

			List<IJsonWriteMiddleware> writeMiddlewares { get; } = new List<IJsonWriteMiddleware>();

			/// <inheritdoc />
			public void AddWriteMiddleware (IJsonWriteMiddleware writeMiddleware) =>
				writeMiddlewares.Add(writeMiddleware);

			/// <inheritdoc />
			public void AddReadMiddleware (IJsonReadMiddleware readMiddleware) =>
				readMiddlewares.Add(readMiddleware);

			WriteJson BuildWrite (Type dataType) {
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

			ReadJson BuildRead (Type dataType) {
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

			public JsonConverter? MayBuildConverter (Type dataType) {
				if (writeMiddlewares.Count > 0 || readMiddlewares.Count > 0) {
					var write = BuildWrite(dataType);
					var read = BuildRead(dataType);
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
				var middlewareContract = contract.Copy();
				var middlewareBuilder = new JsonMiddlewareBuilder();

				foreach (var middlewarePatch in middlewarePatches)
					middlewarePatch.Configure(middlewareContract, middlewareBuilder);

				if (middlewareBuilder.MayBuildConverter(dataType) is {} middlewareConverter) {
					middlewareContract.Converter = middlewareConverter;
					return middlewareContract;
				}
				else
					return contract;
			}
		}

		public MicroContractResolver
		(ImmutableDictionary<Type, IJsonContractCreator> contractCreators,
		 ImmutableArray<IJsonContractFactory> contractFactories,
		 ImmutableArray<IJsonContractPatch> contractPatches,
		 ImmutableArray<IJsonMiddlewarePatch> middlewarePatches) {
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
