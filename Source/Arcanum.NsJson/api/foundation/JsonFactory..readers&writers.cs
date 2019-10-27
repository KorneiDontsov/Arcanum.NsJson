// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.IO;
	using System.Text;

	partial class JsonFactory {
		public static JsonTextReader TextReader (String text) =>
			new JsonTextReader(new StringReader(text));

		public static JsonTextBuilder TextWriter () =>
			new JsonTextBuilder(new StringBuilder());

		public static JsonTextBuilder TextWriter (UInt32 startCapacity) =>
			new JsonTextBuilder(new StringBuilder((Int32) Math.Min(startCapacity, Int32.MaxValue)));

		public static JTokenReader TokenReader (JToken token) =>
			new JTokenReader(token);

		public static JTokenWriter TokenWriter () => new JTokenWriter();
	}
}
