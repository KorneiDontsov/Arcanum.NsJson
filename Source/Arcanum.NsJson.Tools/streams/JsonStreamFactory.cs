// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tools {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.IO;
	using System.Text;

	public static class JsonStreamFactory {
		public static JsonTextReader ReadText (String text) =>
			new JsonTextReader(new StringReader(text));

		public static JsonTextBuilder WriteText () =>
			new JsonTextBuilder(new StringBuilder());

		public static JsonTextBuilder WriteText (UInt32 startCapacity) =>
			new JsonTextBuilder(new StringBuilder((Int32) Math.Min(startCapacity, Int32.MaxValue)));

		public static JTokenReader ReadToken (JToken token) =>
			new JTokenReader(token);

		public static JTokenWriter WriteToken () => new JTokenWriter();
	}
}
