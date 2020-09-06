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
		ImmutableArray<IJsonContractFactory> contractFactories { get; }
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
				if(returnedContract is null)
					returnedContract = contract;
				else
					throw new Exception("Cannot return contract second time.");
			}

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
				foreach(var contractFactory in contractFactories)
					if(RequestFrom(contractFactory) is {} contract)
						return contract;
				return null;
			}

			public JsonContract? RequestFromCompanions () {
				var converterFactories = dataType.EnumerateCompanions<IJsonConverterFactory>();
				foreach(var converterFactory in converterFactories)
					if(RequestFrom(new JsonConverterFactoryAdapter(converterFactory)) is {} contract)
						return contract;
				return null;
			}

			/// <exception cref = "JsonContractException" />
			public JsonContractException NoConverterException () =>
				new JsonContractException($"{dataType} has no converter.");
		}

		/// <summary>
		///     <paramref name = "dataType" /> is not ByRef{T} and not Nullable{T} because execution go through
		///     <see cref = "CreateMiddlewareContract" />.
		/// </summary>
		/// <exception cref = "JsonContractException" />
		JsonContract CreateContract (Type dataType) {
			var contractRequest = new JsonContractRequest(dataType);
			return
				contractRequest.RequestFromCompanions()
				?? contractRequest.RequestFrom(contractFactories)
				?? throw contractRequest.NoConverterException();
		}

		class NullableStructJsonConverter: JsonConverter {
			Type underlyingType { get; }

			public NullableStructJsonConverter (Type underlyingType) =>
				this.underlyingType = underlyingType;

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) => true;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) =>
				throw new JsonSerializationException("Unreachable code: nullable structs are never serialized.");

			/// <inheritdoc />
			public override Object? ReadJson
				(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) =>
				reader.TokenType is JsonToken.Null ? null : serializer.Deserialize(reader, underlyingType);
		}

		static ThreadLocal<Boolean> noMiddleware { get; } = new ThreadLocal<Boolean>();

		class MiddlewareJsonConverter: JsonConverter {
			WriteJson writeJson { get; }
			ReadJson readJson { get; }

			public MiddlewareJsonConverter (WriteJson writeJson, ReadJson readJson) {
				this.writeJson = writeJson;
				this.readJson = readJson;
			}

			/// <inheritdoc />
			public override Boolean CanConvert (Type objectType) => true;

			/// <inheritdoc />
			public override void WriteJson (JsonWriter writer, Object? value, JsonSerializer serializer) {
				if(value is null)
					writer.WriteNull();
				else {
					var arcaneSerializer = serializer.Arcane();
					writeJson(arcaneSerializer, writer, value);
				}
			}

			/// <inheritdoc />
			public override Object? ReadJson
				(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer) {
				if(reader.TokenType is JsonToken.Null)
					return null;
				else {
					var arcaneSerializer = serializer.Arcane();
					return readJson(arcaneSerializer, reader);
				}
			}
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
				foreach(var middlewareFactory in middlewareFactories) middlewareFactory.Handle(this);
				return this;
			}

			public JsonMiddlewareRequest RequestFromCompanions () {
				var middlewareFactories = dataType.EnumerateCompanions<IJsonMiddlewareFactory>();
				foreach(var middlewareFactory in middlewareFactories) middlewareFactory.Handle(this);
				return this;
			}

			WriteJson BuildWrite () {
				WriteJson write =
					(serializer, writer, value) => {
						using(new ThreadLocalTrigger(noMiddleware))
							serializer.Write(writer, value);
					};
				for(var i = toJsonMiddlewares.Count - 1; i >= 0; i -= 1) {
					var middleware = toJsonMiddlewares[i];
					var previous = write;
					write = (serializer, writer, value) => {
						using var localsOwner = serializer.Arcane().CaptureLocals();
						middleware.Write(serializer, writer, value, previous, localsOwner.locals);
					};
				}

				return write;
			}

			ReadJson BuildRead () {
				ReadJson read =
					(serializer, reader) => {
						using(new ThreadLocalTrigger(noMiddleware))
							return serializer.Read(reader, dataType);
					};
				foreach(var middleware in fromJsonMiddlewares) {
					var next = read;
					read = (serializer, reader) => {
						using var localsOwner = serializer.Arcane().CaptureLocals();
						return middleware.Read(serializer, reader, next, localsOwner.locals);
					};
				}

				return read;
			}

			public JsonContract ProduceContract (JsonContract baseContract) {
				if(toJsonMiddlewares.Count > 0 || fromJsonMiddlewares.Count > 0) {
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
			if(dataType.IsByRef && dataType.HasElementType)
				return middlewareContractStorage.GetOrCreate(dataType.GetElementType());
			else if(Nullable.GetUnderlyingType(dataType) is {} underlyingType)
				return
					new JsonLinqContract(underlyingType)
						{ Converter = new NullableStructJsonConverter(underlyingType) };
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
			(ImmutableArray<IJsonContractFactory> contractFactories,
			 ImmutableArray<IJsonMiddlewareFactory> middlewareFactories) {
			this.contractFactories = contractFactories;
			this.middlewareFactories = middlewareFactories;
			contractStorage = new JsonContractStorage(CreateContract);
			middlewareContractStorage = new JsonContractStorage(CreateMiddlewareContract);
		}

		/// <inheritdoc />
		/// <exception cref = "JsonContractException" />
		public JsonContract ResolveContract (Type dataType) {
			var useNoMiddleware = noMiddleware.Value;
			noMiddleware.Value = false;
			return useNoMiddleware
				? contractStorage.GetOrCreate(dataType)
				: middlewareContractStorage.GetOrCreate(dataType);
		}
	}
}
