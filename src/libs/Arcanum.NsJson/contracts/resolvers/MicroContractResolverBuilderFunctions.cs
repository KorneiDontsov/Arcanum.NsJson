// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	public static class MicroContractResolverBuilderFunctions {
		static class Singleton<T> where T: class, new() {
			public static T instance { get; } = new T();
		}

		public static IMicroContractResolverBuilder AddCreator<T> (this IMicroContractResolverBuilder builder)
		where T: class, IJsonContractCreator, new() =>
			builder.AddCreator(Singleton<T>.instance);

		public static IMicroContractResolverBuilder AddFactory<T> (this IMicroContractResolverBuilder builder)
		where T: class, IJsonContractFactory, new() =>
			builder.AddFactory(Singleton<T>.instance);

		public static IMicroContractResolverBuilder AddPatch<T> (this IMicroContractResolverBuilder builder)
		where T: class, IJsonContractPatch, new() =>
			builder.AddPatch(Singleton<T>.instance);

		public static IMicroContractResolverBuilder AddMiddlewarePatch<T> (this IMicroContractResolverBuilder builder)
		where T: class, IJsonMiddlewarePatch, new() =>
			builder.AddMiddlewarePatch(Singleton<T>.instance);
	}
}
