// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	public readonly struct JsonCache : IDisposable
	{
		private MemoryStream _implStream { get; }

		internal JsonCache (MemoryStream implStream)
		{
			_implStream = implStream;
		}

		/// <inheritdoc />
		public void Dispose () => _implStream.Dispose();

		public JsonWriter OpenToWrite ()
		{
			_implStream.Position = 0;

			return new JsonTextWriter(new StreamWriter(_implStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true));
		}

		public JsonReader OpenToRead ()
		{
			_implStream.Position = 0;

			return new JsonTextReader(
				new StreamReader(
					_implStream,
					Encoding.UTF8,
					detectEncodingFromByteOrderMarks: false,
					bufferSize: 1024,
					leaveOpen: true
				)
			);
		}
	}
}
