// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.IO;
	using System.Text;

	public static class JsonStreamModule {
		public static JsonTextReader ReadTextJson (String text) =>
			new JsonTextReader(new StringReader(text));

		public static JsonTextBuilder WriteTextJson () =>
			new JsonTextBuilder(new StringBuilder());

		public static JsonTextBuilder WriteTextJson (UInt32 startCapacity) =>
			new JsonTextBuilder(new StringBuilder((Int32) Math.Min(startCapacity, Int32.MaxValue)));

		public static JTokenReader ReadTokenJson (JToken token) =>
			new JTokenReader(token);

		public static JTokenWriter WriteTokenJson () => new JTokenWriter();
	}
}
