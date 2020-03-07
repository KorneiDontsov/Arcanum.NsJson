// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Contracts {
	using System;

	public interface IJsonConverterRequest {
		Type dataType { get; }

		void Return (IJsonConverter jsonConverter);

		void ReturnReadOnly (IFromJsonConverter fromJsonConverter);

		void ReturnWriteOnly (IToJsonConverter toJsonConverter);
	}
}
