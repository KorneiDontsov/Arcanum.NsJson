// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Microsoft.IO;
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Text;

	readonly struct JsonCache: IDisposable {
		MemoryStream stream { get; }

		JsonCache (MemoryStream stream) => this.stream = stream;

		/// <inheritdoc />
		public void Dispose () => stream.Dispose();

		static RecyclableMemoryStreamManager streamManager { get; } = new RecyclableMemoryStreamManager();

		public static JsonCache Rent () => new JsonCache(streamManager.GetStream());

		public JsonWriter OpenToWrite () {
			stream.Position = 0;
			return new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true));
		}

		public JsonReader OpenToRead () {
			stream.Position = 0;
			var reader =
				new JsonTextReader(
					new StreamReader(
						stream,
						Encoding.UTF8,
						detectEncodingFromByteOrderMarks: false,
						bufferSize: 1024,
						leaveOpen: true
					));
			reader.Read();
			return reader;
		}
	}
}
