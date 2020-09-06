// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using Newtonsoft.Json;
	using System;
	using static Arcanum.DataContracts.Module;

	public sealed class SamplesJsonMiddlewareFactory: IJsonMiddlewareFactory {
		class SamplesJsonMiddleware: IFromJsonMiddleware {
			readonly Type dataType;
			readonly LocalSamples? baseSamples;

			public SamplesJsonMiddleware (Type dataType, LocalSamples? baseSamples) {
				this.dataType = dataType;
				this.baseSamples = baseSamples;
			}

			/// <inheritdoc />
			public Object Read
				(IJsonSerializer serializer, JsonReader reader, ReadJson next, ILocalsCollection locals) {
				if(! (locals.MaybeSamples() is {} actualSamples)
				   || actualSamples.dataType is {} && actualSamples.dataType != dataType)
					locals.SetSamples(baseSamples);
				else if(baseSamples is {})
					locals.SetSamples(baseSamples.Override(actualSamples));
				else if(actualSamples.dataType is null)
					locals.SetSamples(
						new LocalSamples(
							dataType,
							createSelfSample: actualSamples.createSelfSample,
							createItemSample: actualSamples.createItemSample,
							actualSamples.createMemberSamples));

				return next(serializer, reader);
			}
		}

		/// <inheritdoc />
		public void Handle (IJsonMiddlewareRequest request) {
			static LocalSamples? GetSamples (Type dataType) {
				if(! (dataType.IsClass || dataType.IsValueType)
				   || dataType.IsGenericTypeDefinition
				   || ! (GetDataTypeInfo(dataType).contract.mayPrepareSamples is {} prepareSamples))
					return null;
				else {
					var samplesBuilder = new LocalSamplesBuilder();
					try {
						prepareSamples(samplesBuilder);
					}
					catch(Exception ex) {
						throw new JsonContractException($"Failed to prepare samples of type {dataType}.", ex);
					}

					if(samplesBuilder.Build() is {isEmpty: false} builtSamples)
						return builtSamples;
					else
						return null;
				}
			}

			var samples = GetSamples(request.dataType);
			request.Yield(new SamplesJsonMiddleware(request.dataType, samples));
		}
	}
}
