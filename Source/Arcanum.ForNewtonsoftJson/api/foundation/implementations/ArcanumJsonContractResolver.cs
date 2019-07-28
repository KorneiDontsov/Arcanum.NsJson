// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;

using Newtonsoft.Json.Serialization;

namespace Arcanum.ForNewtonsoftJson
{
	public class ArcanumJsonContractResolver : DefaultContractResolver
	{
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

		private sealed class MiddlewareContractStorage
		{
			private ConcurrentDictionary<Type, JsonContract> _dict { get; }

			private Func<Type, JsonContract> _contractCreator { get; }

			public MiddlewareContractStorage (ArcanumJsonContractResolver resolver)
			{
				_dict = new ConcurrentDictionary<Type, JsonContract>();
				_contractCreator = resolver.CreateMiddlewareContract;
			}

			public JsonContract Get (Type type) => _dict.GetOrAdd(type, _contractCreator);
		}

		private MiddlewareContractStorage _middlewareContractStorage { get; }

		public ArcanumJsonContractResolver ()
		{
			_middlewareContractStorage = new MiddlewareContractStorage(this);

			configure();
		}

		private void configure ()
		{
			SerializeCompilerGeneratedMembers = true;
		}

		/// <inheritdoc />
		public sealed override JsonContract ResolveContract (Type type)
		{
			var args = ResolveContractArgs.Pick();

			return args.withoutMiddleware
			? base.ResolveContract(type)
			: _middlewareContractStorage.Get(type);
		}

		private JsonContract CreateMiddlewareContract (Type objectType)
		{
			var baseContract = base.ResolveContract(objectType);
			return ApplyMiddleware(baseContract);
		}

		protected virtual JsonContract ApplyMiddleware (JsonContract contract) => contract;

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
