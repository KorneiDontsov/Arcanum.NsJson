// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.Tools {
	using Newtonsoft.Json;
	using System;
	using System.Globalization;

	public static class JsonExceptionFactory {
		// based on
		// https://github.com/JamesNK/Newtonsoft.Json/blob/febdb8188b226ee7810bbf7e053ada171818fbeb/Src/Newtonsoft.Json/JsonReaderException.cs
		// https://github.com/JamesNK/Newtonsoft.Json/blob/6360cc4c4b4a45f7a82ad6c060cddd9cdb3c7d02/Src/Newtonsoft.Json/JsonPosition.cs
		// 
		// Copyright (c) 2007 James Newton-King

		public static JsonReaderException ReaderException (
		IJsonLineInfo? maybeLineInfo, String path, String message, Exception? innerException = null) {
			static String FormatMessage (IJsonLineInfo? maybeAvailableLineInfo, String path, String message) {
				if (! message.EndsWith(Environment.NewLine, StringComparison.Ordinal)) {
					message = message.Trim();

					if (! message.EndsWith(".", StringComparison.Ordinal)) message += ".";

					message += " ";
				}

				message += String.Format(CultureInfo.InvariantCulture, "Path '{0}'", path);

				if (maybeAvailableLineInfo is { } lineInfo && lineInfo.HasLineInfo())
					message += String.Format(
						CultureInfo.InvariantCulture,
						", line {0}, position {1}",
						maybeAvailableLineInfo.LineNumber,
						maybeAvailableLineInfo.LinePosition
					);

				message += ".";

				return message;
			}

			var maybeAvailableLineInfo = maybeLineInfo?.HasLineInfo() is true ? maybeLineInfo : null;
			var formattedMessage = FormatMessage(maybeAvailableLineInfo, path, message);
			var (lineNumber, linePosition) =
				maybeAvailableLineInfo is { } lineInfo
					? (lineInfo.LineNumber, lineInfo.LinePosition)
					: (0, 0);
			return new JsonReaderException(formattedMessage, path, lineNumber, linePosition, innerException);
		}

		public static JsonReaderException ReaderException (
		JsonReader jsonReader, String message, Exception? innerException) =>
			ReaderException(jsonReader as IJsonLineInfo, jsonReader.Path, message, innerException);

		public static JsonReaderException ReaderException (JsonReader jsonReader, String message) =>
			ReaderException(jsonReader, message, innerException: null);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
		JsonReader jsonReader, IFormatProvider formatProvider, String messageFormat, params Object[] messageArgs) =>
			ReaderException(
				jsonReader,
				String.Format(formatProvider, messageFormat, messageArgs),
				innerException: null);

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
		JsonReader jsonReader, String messageFormat, params Object[] messageArgs) =>
			ReaderException(jsonReader, CultureInfo.InvariantCulture, messageFormat, messageArgs);
	}
}
