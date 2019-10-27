// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;
	using System.Threading;

	readonly struct MiddlewaredContractResolverArgs {
		internal readonly struct Scope: IDisposable {
			/// <inheritdoc />
			public void Dispose () => local.Value = default;
		}

		public Boolean withoutMiddleware { get; }

		static AsyncLocal<MiddlewaredContractResolverArgs> local { get; }
			= new AsyncLocal<MiddlewaredContractResolverArgs>();

		MiddlewaredContractResolverArgs (Boolean withoutMiddleware) => this.withoutMiddleware = withoutMiddleware;

		public static Scope Set (Boolean withoutMiddleware) {
			local.Value = new MiddlewaredContractResolverArgs(withoutMiddleware);
			return new Scope();
		}

		public static MiddlewaredContractResolverArgs Pick () {
			var args = local.Value;
			local.Value = default;
			return args;
		}
	}
}
