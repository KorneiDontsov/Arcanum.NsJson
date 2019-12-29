// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tools {
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Text;

	public sealed class JsonTextBuilder: JsonTextWriter {
		StringBuilder sb { get; }

		/// <inheritdoc />
		internal JsonTextBuilder (StringBuilder sb): base(new StringWriter(sb)) => this.sb = sb;

		/// <inheritdoc />
		/// <summary>
		///     Returns json text that is built.
		/// </summary>
		public override String ToString () => sb.ToString();
	}
}
