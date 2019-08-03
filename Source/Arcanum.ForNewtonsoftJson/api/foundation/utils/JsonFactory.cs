// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Globalization;

using Arcanum.ForNewtonsoftJson.Annotations;

using Newtonsoft.Json;

namespace Arcanum.ForNewtonsoftJson
{
	public static class JsonFactory
	{
		#region exceptions
		// based on
		// https://github.com/JamesNK/Newtonsoft.Json/blob/febdb8188b226ee7810bbf7e053ada171818fbeb/Src/Newtonsoft.Json/JsonReaderException.cs
		// https://github.com/JamesNK/Newtonsoft.Json/blob/6360cc4c4b4a45f7a82ad6c060cddd9cdb3c7d02/Src/Newtonsoft.Json/JsonPosition.cs
		// 
		// Copyright (c) 2007 James Newton-King

		public static JsonReaderException ReaderException (
			IJsonLineInfo? maybeLineInfo,
			String path,
			String message,
			Exception? innerException = null
		)
		{
			message = formatMessage(maybeLineInfo, path, message);

			Int32 lineNumber;
			Int32 linePosition;
			if (maybeLineInfo != null && maybeLineInfo.HasLineInfo())
			{
				lineNumber = maybeLineInfo.LineNumber;
				linePosition = maybeLineInfo.LinePosition;
			}
			else
			{
				lineNumber = 0;
				linePosition = 0;
			}

			return new JsonReaderException(message, path, lineNumber, linePosition, innerException);

			static String formatMessage (IJsonLineInfo? maybeLineInfo, String path, String message)
			{
				if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
				{
					message = message.Trim();

					if (!message.EndsWith(".", StringComparison.Ordinal))
					{
						message += ".";
					}

					message += " ";
				}

				message += String.Format(CultureInfo.InvariantCulture, "Path '{0}'", path);

				if (maybeLineInfo != null && maybeLineInfo.HasLineInfo())
				{
					message += String.Format(
						CultureInfo.InvariantCulture,
						", line {0}, position {1}",
						maybeLineInfo.LineNumber,
						maybeLineInfo.LinePosition
					);
				}

				message += ".";

				return message;
			}
		}

		public static JsonReaderException ReaderException (
			JsonReader jsonReader,
			String message,
			Exception? innerException
		)
		{
			return ReaderException(jsonReader as IJsonLineInfo, jsonReader.Path, message, innerException);
		}

		public static JsonReaderException ReaderException (JsonReader jsonReader, String message)
		{
			return ReaderException(jsonReader, message, innerException: null);
		}

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
			JsonReader jsonReader,
			IFormatProvider formatProvider,
			String messageFormat,
			params Object[] messageArgs
		)
		{
			return ReaderException(
				jsonReader,
				String.Format(formatProvider, messageFormat, messageArgs),
				innerException: null
			);
		}

		[StringFormatMethod("messageFormat")]
		public static JsonReaderException ReaderException (
			JsonReader jsonReader,
			String messageFormat,
			params Object[] messageArgs
		)
		{
			return ReaderException(jsonReader, CultureInfo.InvariantCulture, messageFormat, messageArgs);
		}
		#endregion
	}
}
