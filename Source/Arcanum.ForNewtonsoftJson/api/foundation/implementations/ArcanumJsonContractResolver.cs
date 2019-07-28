// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;

using Arcanum.DataContracts;

using Newtonsoft.Json.Serialization;

namespace Arcanum.ForNewtonsoftJson
{
	public class ArcanumJsonContractResolver : DefaultContractResolver
	{
		public sealed class Dependencies
		{
			public IDataTypeInfoStorage dataTypeInfoStorage { get; }

			public static Dependencies @default { get; } = new Dependencies(DataTypeInfoStorage.shared);

			public Dependencies (IDataTypeInfoStorage dataTypeInfoStorage)
			{
				this.dataTypeInfoStorage = dataTypeInfoStorage;
			}
		}

		protected Dependencies dependencies { get; }

		private MiddlewareContractStorage _middlewareContractStorage { get; }

		public ArcanumJsonContractResolver (Dependencies dependencies)
		{
			this.dependencies = dependencies;
			_middlewareContractStorage = new MiddlewareContractStorage(this);

			configure();
		}

		public ArcanumJsonContractResolver () : this(Dependencies.@default) { }

		private void configure ()
		{
			SerializeCompilerGeneratedMembers = true;
		}

		#region resolve
		private sealed class MiddlewareContractStorage
		{
			private ConcurrentDictionary<Type, JsonContract> _dict { get; }

			private Func<Type, JsonContract> _contractCreator { get; }

			public MiddlewareContractStorage (ArcanumJsonContractResolver resolver)
			{
				_dict = new ConcurrentDictionary<Type, JsonContract>();
				_contractCreator = resolver.ConstructMiddlewareContract;
			}

			public JsonContract Get (Type type) => _dict.GetOrAdd(type, _contractCreator);
		}

		/// <inheritdoc />
		internal readonly struct ResolveContractArgs
		{
			internal readonly struct Scope : IDisposable
			{
				/// <inheritdoc />
				public void Dispose () => _local.Value = default;
			}

			public Boolean withoutMiddleware { get; }

			private static AsyncLocal<ResolveContractArgs> _local { get; }
				= new AsyncLocal<ResolveContractArgs>();

			private ResolveContractArgs (Boolean withoutMiddleware) => this.withoutMiddleware = withoutMiddleware;

			public static Scope Set (Boolean withoutMiddleware)
			{
				_local.Value = new ResolveContractArgs(withoutMiddleware);

				return default;
			}

			public static ResolveContractArgs Pick ()
			{
				var args = _local.Value;
				_local.Value = default;

				return args;
			}
		}

		public sealed override JsonContract ResolveContract (Type type)
		{
			var args = ResolveContractArgs.Pick();

			return args.withoutMiddleware
			? ResolveContractWithoutMiddleware(type)
			: ResolveContractWithMiddleware(type);
		}

		protected JsonContract ResolveContractWithoutMiddleware (Type type)
		{
			return base.ResolveContract(type);
		}

		protected JsonContract ResolveContractWithMiddleware (Type type)
		{
			return _middlewareContractStorage.Get(type);
		}

		private JsonContract ConstructMiddlewareContract (Type objectType)
		{
			var baseContract = ResolveContractWithoutMiddleware(objectType);
			return CreateMiddlewareContract(baseContract);
		}

		protected virtual JsonContract CreateMiddlewareContract (JsonContract contract)
		{
			if (contract.UnderlyingType.IsClass)
			{
				if (contract.Converter is null && contract.UnderlyingType.IsAbstract)
				{
					var maybeDiscriminatedUnionInfo = dependencies.dataTypeInfoStorage.Get(contract.UnderlyingType)
					.asDiscriminatedUnionInfo;

					if (maybeDiscriminatedUnionInfo != null)
					{
						return CreateDiscriminatedUnionMiddlewareContract(contract, maybeDiscriminatedUnionInfo);
					}
				}
			}

			return contract;
		}

		/// <param name="discriminatedUnionInfo">
		///     Must be of type <see cref="JsonContract.UnderlyingType" /> specified in
		///     <paramref name="contract" />.
		/// </param>
		protected virtual JsonContract CreateDiscriminatedUnionMiddlewareContract (
			JsonContract contract,
			DiscriminatedUnionInfo discriminatedUnionInfo
		)
		{
			if (contract.UnderlyingType != discriminatedUnionInfo.dataTypeInfo.dataType)
			{
				throw new Exception(
					$"'{nameof(discriminatedUnionInfo)}' {discriminatedUnionInfo} doesn't correspond to 'contract' of type {contract.UnderlyingType}."
				);
			}
			else if (discriminatedUnionInfo.hasErrors)
			{
				throw new JsonContractResolveException(
					$"Cannot resolve {discriminatedUnionInfo.dataType} because it has errors.\n{discriminatedUnionInfo.GetErrorString()}"
				);
			}

			var middlewareContract = contract.Copy();
			middlewareContract.Converter = new DiscriminatedUnionJsonReadConverter(discriminatedUnionInfo);

			return middlewareContract;
		}
		#endregion

		#region shared
		private sealed class Immutable : IContractResolver
		{
			private ArcanumJsonContractResolver _core { get; }

			public Immutable ()
			{
				_core = new ArcanumJsonContractResolver();
			}

			/// <inheritdoc />
			public JsonContract ResolveContract (Type type) => _core.ResolveContract(type);
		}

		public static IContractResolver shared { get; } = new Immutable();
		#endregion
	}
}
