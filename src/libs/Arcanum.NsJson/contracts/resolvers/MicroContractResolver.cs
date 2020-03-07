// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Arcanum.Companions;
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
		ImmutableArray<IJsonMiddlewareFactory> middlewareFactories { get; }
		JsonContractStorage contractStorage { get; }
		JsonContractStorage middlewareContractStorage { get; }

		class JsonContractRequest: IJsonContractRequest {
			/// <inheritdoc />
			public Type dataType { get; }

			JsonContract? returnedContract;

			public JsonContractRequest (Type dataType) => this.dataType = dataType;

			/// <inheritdoc />
			public void Return (JsonContract contract) {
				if (returnedContract is null)
					returnedContract = contract;
				else
					throw new Exception("Cannot return contract second time.");
			}

			JsonContract Create (IJsonContractCreator contractCreator) {
				var contract = contractCreator.CreateJsonContract();
				if (contract.UnderlyingType == dataType)
					return contract;
				else {
					var msg =
						$"Json contract creator '{contractCreator.GetType()}' created contract of type "
						+ $"'{contract.UnderlyingType}' instead of '{dataType}'.";
					throw new Exception(msg);
				}
			}

			public JsonContract? MayCreate (ImmutableDictionary<Type, IJsonContractCreator> contractCreators) =>
				contractCreators.TryGetValue(dataType, out var contractCreator)
					? Create(contractCreator)
					: null;

			public JsonContract? MayCreateFromCompanion () =>
				dataType.MayGetCompanion<IJsonContractCreator>() is {} contractCreator
					? Create(contractCreator)
					: null;

			Exception WrongContractTypeError (IJsonContractFactory contractFactory, JsonContract contract) {
				var msg =
					$"Json contract factory '{contractFactory.GetType()}' created contract of type "
					+ $"'{contract.UnderlyingType}' instead of '{dataType}'.";
				throw new Exception(msg);
			}

			JsonContract? RequestFrom (IJsonContractFactory contractFactory) {
				returnedContract = null;
				contractFactory.Handle(this);
				return
					returnedContract switch {
						{} contract when contract.UnderlyingType == dataType => contract,
						{} contract => throw WrongContractTypeError(contractFactory, contract),
						_ => null
					};
			}

			public JsonContract? RequestFrom (ImmutableArray<IJsonContractFactory> contractFactories) {
				foreach (var contractFactory in contractFactories)
					if (RequestFrom(contractFactory) is {} contract)
						return contract;
				return null;
			}

			public JsonContract? RequestFromCompanions () {
				var contractFactories = dataType.EnumerateCompanions<IJsonContractFactory>();
				foreach (var contractFactory in contractFactories)
					if (RequestFrom(contractFactory) is {} contract)
						return contract;
				return null;
			}

			/// <exception cref = "JsonContractException" />
			public JsonContractException NoContractException () =>
				new JsonContractException($"{dataType} has no contract.");
		}

		/// <summary>
		///     <paramref name = "dataType" /> is not ByRef type because execution go through
		///     <see cref = "CreateMiddlewareContract" />.
		/// </summary>
		/// <exception cref = "JsonContractException" />
		JsonContract CreateContract (Type dataType) {
			var contractRequest = new JsonContractRequest(dataType);
			var contract =
				contractRequest.MayCreateFromCompanion()
				?? contractRequest.RequestFromCompanions()
				?? contractRequest.MayCreate(contractCreators)
				?? contractRequest.RequestFrom(contractFactories)
				?? throw contractRequest.NoContractException();
			foreach (var contractPatch in contractPatches) contractPatch.Patch(contract);
			return contract;
		}

		static ThreadLocal<Boolean> noMiddleware { get; } = new ThreadLocal<Boolean>();

		class MiddlewareJsonConverter: JsonConverterAdapter, IToJsonConverter, IFromJsonConverter {
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
			public Object? Read (JsonReader reader, JsonSerializer serializer) =>
				readJson(reader, serializer);
		}

		class JsonMiddlewareRequest: IJsonMiddlewareRequest {
			List<IFromJsonMiddleware> fromJsonMiddlewares { get; } = new List<IFromJsonMiddleware>();

			List<IToJsonMiddleware> toJsonMiddlewares { get; } = new List<IToJsonMiddleware>();

			/// <inheritdoc />
			public Type dataType { get; }

			public JsonMiddlewareRequest (Type dataType) => this.dataType = dataType;

			/// <inheritdoc />
			public void Yield (IToJsonMiddleware toJsonMiddleware) =>
				toJsonMiddlewares.Add(toJsonMiddleware);

			/// <inheritdoc />
			public void Yield (IFromJsonMiddleware fromJsonMiddleware) =>
				fromJsonMiddlewares.Add(fromJsonMiddleware);

			public JsonMiddlewareRequest RequestFrom (ImmutableArray<IJsonMiddlewareFactory> middlewareFactories) {
				foreach (var middlewareFactory in middlewareFactories) middlewareFactory.Handle(this);
				return this;
			}

			public JsonMiddlewareRequest RequestFromCompanions () {
				var middlewareFactories = dataType.EnumerateCompanions<IJsonMiddlewareFactory>();
				foreach (var middlewareFactory in middlewareFactories) middlewareFactory.Handle(this);
				return this;
			}

			WriteJson BuildWrite () {
				WriteJson write =
					(writer, value, serializer) => {
						using (new ThreadLocalTrigger(noMiddleware))
							serializer.Serialize(writer, value, dataType);
					};
				for (var i = toJsonMiddlewares.Count - 1; i >= 0; i -= 1) {
					var middleware = toJsonMiddlewares[i];
					var previous = write;
					write = (writer, value, serializer) => middleware.Write(writer, value, serializer, previous);
				}

				return write;
			}

			ReadJson BuildRead () {
				ReadJson read =
					(reader, serializer) => {
						using (new ThreadLocalTrigger(noMiddleware))
							return serializer.Deserialize(reader, dataType);
					};
				foreach (var middleware in fromJsonMiddlewares) {
					var next = read;
					read = (reader, serializer) => middleware.Read(reader, serializer, next);
				}

				return read;
			}

			public JsonContract ProduceContract (JsonContract baseContract) {
				if (toJsonMiddlewares.Count > 0 || fromJsonMiddlewares.Count > 0) {
					var write = BuildWrite();
					var read = BuildRead();
					var mwConverter = new MiddlewareJsonConverter(write, read);
					return baseContract.Copy().AddConverter(mwConverter);
				}
				else
					return baseContract;
			}
		}

		/// <exception cref = "JsonContractException" />
		JsonContract CreateMiddlewareContract (Type dataType) {
			if (dataType.IsByRef && dataType.HasElementType)
				return middlewareContractStorage.GetOrCreate(dataType.GetElementType());
			else {
				var baseContract = contractStorage.GetOrCreate(dataType);
				return
					new JsonMiddlewareRequest(dataType)
						.RequestFromCompanions()
						.RequestFrom(middlewareFactories)
						.ProduceContract(baseContract);
			}
		}

		public MicroContractResolver
			(ImmutableDictionary<Type, IJsonContractCreator> contractCreators,
			 ImmutableArray<IJsonContractFactory> contractFactories,
			 ImmutableArray<IJsonContractPatch> contractPatches,
			 ImmutableArray<IJsonMiddlewareFactory> middlewareFactories) {
			this.contractCreators = contractCreators;
			this.contractFactories = contractFactories;
			this.contractPatches = contractPatches;
			this.middlewareFactories = middlewareFactories;
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
