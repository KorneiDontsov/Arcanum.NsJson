// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using System;
	using System.Threading;

	readonly struct JsonContractResolveArgs {
		internal readonly struct Scope: IDisposable {
			/// <inheritdoc />
			public void Dispose () => local.Value = default;
		}

		public Boolean withoutMiddleware { get; }

		static AsyncLocal<JsonContractResolveArgs> local { get; }
			= new AsyncLocal<JsonContractResolveArgs>();

		JsonContractResolveArgs (Boolean withoutMiddleware) => this.withoutMiddleware = withoutMiddleware;

		public static Scope Set (Boolean withoutMiddleware) {
			local.Value = new JsonContractResolveArgs(withoutMiddleware);
			return new Scope();
		}

		public static JsonContractResolveArgs Pick () {
			var args = local.Value;
			local.Value = default;
			return args;
		}
	}
}
