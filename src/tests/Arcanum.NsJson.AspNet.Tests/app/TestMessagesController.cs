// Copyright (c) Kornei Dontsov. All Rights Reserved. Licensed under the MIT. See LICENSE in the project root for license information.

namespace Arcanum.NsJson.AspNet.Tests {
	using Microsoft.AspNetCore.Mvc;
	using System;

	public class PostMessageRequest {
		public String author { get; }
		public String message { get; }

		public PostMessageRequest (String author, String message) {
			this.author = author;
			this.message = message;
		}
	}

	public enum MessageKind { Test }

	public class PostMessageResponse {
		public String author { get; }
		public String message { get; }
		public MessageKind kind { get; }

		public PostMessageResponse (String author, String message, MessageKind kind) {
			this.author = author;
			this.message = message;
			this.kind = kind;
		}
	}

	[ApiController, Route("/")]
	public class TestMessagesController: ControllerBase {
		[Route("/test_messages")]
		public PostMessageResponse Post (PostMessageRequest request) =>
			new PostMessageResponse(request.author, request.message, MessageKind.Test);
	}
}
