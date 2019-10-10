// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.ForNewtonsoftJson {
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Text;

	public readonly struct JsonCache: IDisposable {
		MemoryStream implStream { get; }

		internal JsonCache (MemoryStream implStream) => this.implStream = implStream;

		/// <inheritdoc />
		public void Dispose () => implStream.Dispose();

		public JsonWriter OpenToWrite () {
			implStream.Position = 0;
			return new JsonTextWriter(new StreamWriter(implStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true));
		}

		public JsonReader OpenToRead () {
			implStream.Position = 0;

			var reader = new JsonTextReader(
				new StreamReader(
					implStream,
					Encoding.UTF8,
					detectEncodingFromByteOrderMarks: false,
					bufferSize: 1024,
					leaveOpen: true
				)
			);
			reader.Read();

			return reader;
		}
	}
}
