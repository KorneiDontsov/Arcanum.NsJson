// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.ForNewtonsoftJson {
	using DataContracts;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Concurrent;
	using System.Threading;

	public class ArcanumJsonContractResolver: DefaultContractResolver {
		public sealed class Dependencies {
			public IDataTypeInfoFactory dataTypeInfoFactory { get; }

			public static Dependencies @default { get; } = new Dependencies(DataTypeInfoStorage.shared);

			public Dependencies (IDataTypeInfoFactory dataTypeInfoFactory) =>
				this.dataTypeInfoFactory = dataTypeInfoFactory;
		}

		protected Dependencies dependencies { get; }

		MiddlewareContractStorage middlewareContractStorage { get; }

		public ArcanumJsonContractResolver (Dependencies dependencies) {
			this.dependencies = dependencies;
			middlewareContractStorage = new MiddlewareContractStorage(this);

			Configure();
		}

		public ArcanumJsonContractResolver (): this(Dependencies.@default) { }

		void Configure () =>
			SerializeCompilerGeneratedMembers = true;

		#region resolve
		sealed class MiddlewareContractStorage {
			ConcurrentDictionary<Type, JsonContract> dict { get; }

			Func<Type, JsonContract> contractCreator { get; }

			public MiddlewareContractStorage (ArcanumJsonContractResolver resolver) {
				dict = new ConcurrentDictionary<Type, JsonContract>();
				contractCreator = resolver.ConstructMiddlewareContract;
			}

			public JsonContract Get (Type type) => dict.GetOrAdd(type, contractCreator);
		}

		/// <inheritdoc />
		internal readonly struct ResolveContractArgs {
			internal readonly struct Scope: IDisposable {
				/// <inheritdoc />
				public void Dispose () => local.Value = default;
			}

			public Boolean withoutMiddleware { get; }

			static AsyncLocal<ResolveContractArgs> local { get; }
				= new AsyncLocal<ResolveContractArgs>();

			ResolveContractArgs (Boolean withoutMiddleware) => this.withoutMiddleware = withoutMiddleware;

			public static Scope Set (Boolean withoutMiddleware) {
				local.Value = new ResolveContractArgs(withoutMiddleware);

				return default;
			}

			public static ResolveContractArgs Pick () {
				var args = local.Value;
				local.Value = default;

				return args;
			}
		}

		public override sealed JsonContract ResolveContract (Type type) {
			var args = ResolveContractArgs.Pick();

			return args.withoutMiddleware
				? ResolveContractWithoutMiddleware(type)
				: ResolveContractWithMiddleware(type);
		}

		protected JsonContract ResolveContractWithoutMiddleware (Type type) =>
			base.ResolveContract(type);

		protected JsonContract ResolveContractWithMiddleware (Type type) =>
			middlewareContractStorage.Get(type);

		JsonContract ConstructMiddlewareContract (Type objectType) {
			var baseContract = ResolveContractWithoutMiddleware(objectType);
			return CreateMiddlewareContract(baseContract);
		}

		protected virtual JsonContract CreateMiddlewareContract (JsonContract contract) {
			static Boolean IsOfAbstractClass (JsonContract contract) =>
				contract.UnderlyingType.IsClass && contract.UnderlyingType.IsAbstract;

			static Boolean IsOfNonAbstractClass (JsonContract contract) =>
				contract.UnderlyingType.IsClass && ! contract.UnderlyingType.IsAbstract;

			static Boolean HasNoConverter (JsonContract contract) =>
				contract.Converter is null;

			static IUnionInfo? TryGetUnionInfo (IDataTypeInfoFactory infoFactory, JsonContract contract) =>
				infoFactory.Get(contract.UnderlyingType).asUnionInfo;

			static IUnionCaseInfo? TryGetUnionCaseInfo (IDataTypeInfoFactory infoFactory, JsonContract contract) =>
				infoFactory.Get(contract.UnderlyingType).asUnionCaseInfo;

			if (IsOfAbstractClass(contract)
			&& HasNoConverter(contract)
			&& TryGetUnionInfo(dependencies.dataTypeInfoFactory, contract) is { } unionInfo)
				return CreateUnionMiddlewareContract(contract, unionInfo);
			else if (IsOfNonAbstractClass(contract)
			&& TryGetUnionCaseInfo(dependencies.dataTypeInfoFactory, contract) is { } unionCaseInfo)
				return CreateUnionCaseMiddlewareContract(contract, unionCaseInfo);
			else
				return contract;
		}

		/// <param name = "unionInfo">
		///     Must be of type <see cref = "JsonContract.UnderlyingType" /> specified in
		///     <paramref name = "contract" />.
		/// </param>
		protected virtual JsonContract CreateUnionMiddlewareContract (JsonContract contract, IUnionInfo unionInfo) {
			if (contract.UnderlyingType != unionInfo.dataType)
				throw
					new Exception(
						$"'{nameof(unionInfo)}' {unionInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}.");
			else if (unionInfo.hasErrors)
				throw
					new JsonContractResolveException(
						$"Cannot resolve {unionInfo.dataType} because it has errors.\n{unionInfo.GetErrorString()}");

			var middlewareContract = contract.Copy();
			middlewareContract.Converter = new UnionJsonReadConverter(unionInfo);

			return middlewareContract;
		}

		/// <param name = "unionCaseInfo">
		///     Must be of type <see cref = "JsonContract.UnderlyingType" /> specified in
		///     <paramref name = "contract" />.
		/// </param>
		protected virtual JsonContract CreateUnionCaseMiddlewareContract (
		JsonContract contract, IUnionCaseInfo unionCaseInfo) {
			if (contract.UnderlyingType != unionCaseInfo.dataType)
				throw
					new Exception(
						$"'{nameof(unionCaseInfo)}' {unionCaseInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}.");

			var middlewareContract = contract.Copy();
			middlewareContract.Converter = new UnionCaseJsonConverter(unionCaseInfo);

			return middlewareContract;
		}
		#endregion

		#region shared
		sealed class Immutable: IContractResolver {
			ArcanumJsonContractResolver core { get; }

			public Immutable () => core = new ArcanumJsonContractResolver();

			/// <inheritdoc />
			public JsonContract ResolveContract (Type type) => core.ResolveContract(type);
		}

		public static IContractResolver shared { get; } = new Immutable();
		#endregion
	}
}
