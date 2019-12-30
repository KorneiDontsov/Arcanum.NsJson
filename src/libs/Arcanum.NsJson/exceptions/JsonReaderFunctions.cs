// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson {
	using Newtonsoft.Json;
	using System;
	using static Arcanum.NsJson.JsonExceptionModule;

	public static class JsonReaderFunctions {
		public static JsonReaderException Exception
		(this JsonReader jsonReader,
		 String message,
		 Exception? innerException) =>
			ReaderException(jsonReader, message, innerException);

		public static JsonReaderException Exception (this JsonReader jsonReader, String message) =>
			ReaderException(jsonReader, message);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException Exception
		(this JsonReader jsonReader,
		 IFormatProvider formatProvider,
		 String messageFormat,
		 params Object[] messageArgs) =>
			ReaderException(jsonReader, formatProvider, messageFormat, messageArgs);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException Exception
		(this JsonReader jsonReader,
		 String messageFormat,
		 params Object[] messageArgs) =>
			ReaderException(jsonReader, messageFormat, messageArgs);

		public static void ReadNext (this JsonReader jsonReader) {
			if (! jsonReader.Read()) throw jsonReader.Exception("Unexpected end when reading JSON.");
		}

		public static void CurrentTokenMustBe (this JsonReader jsonReader, JsonToken expected) {
			if (jsonReader.TokenType != expected)
				throw
					jsonReader.Exception("Expected token to be {0} but accepted {1}.", expected, jsonReader.TokenType);
		}
	}
}
