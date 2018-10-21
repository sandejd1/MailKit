﻿//
// EnvelopeTests.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2018 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;

using NUnit.Framework;

using MimeKit;
using MimeKit.Utils;

using MailKit;

namespace UnitTests
{
	[TestFixture]
	public class EnvelopeTests
	{
		[Test]
		public void TestArgumentExceptions ()
		{
			Envelope envelope;

			Assert.Throws<ArgumentNullException> (() => Envelope.TryParse (null, out envelope));
		}

		[Test]
		public void TestSerialization ()
		{
			var original = new Envelope ();
			original.Bcc.Add (new MailboxAddress ("Bcc Recipient", "unit-tests@mimekit.org"));
			original.Cc.Add (new MailboxAddress ("Routed Mailbox", new string[] { "domain.org" }, "unit-tests@mimekit.org"));
			original.Date = DateTimeOffset.Now;
			original.From.Add (new MailboxAddress ("MailKit Unit Tests", "unit-tests@mimekit.org"));
			original.InReplyTo = "<xyz@mimekit.org>";
			original.MessageId = "<xyz123@mimekit.org>";
			original.ReplyTo.Add (new MailboxAddress ("Reply-To", "unit-tests@mimekit.org"));
			original.Sender.Add (new MailboxAddress ("The Real Sender", "unit-tests@mimekit.org"));
			original.Subject = "This is the subject";
			original.To.Add (new GroupAddress ("Group Address", new MailboxAddress[] {
				new MailboxAddress ("Recipient 1", "unit-tests@mimekit.org"),
				new MailboxAddress ("Recipient 2", "unit-tests@mimekit.org")
			}));
			var text = original.ToString ();
			Envelope envelope;

			Assert.IsTrue (Envelope.TryParse (text, out envelope));
			var text2 = envelope.ToString ();

			Assert.AreEqual (text, text2);
		}

		[Test]
		public void TestExampleEnvelopeRfc3501 ()
		{
			const string text = "(\"Wed, 17 Jul 1996 02:23:25 -0700 (PDT)\" \"IMAP4rev1 WG mtg summary and minutes\" ((\"Terry Gray\" NIL \"gray\" \"cac.washington.edu\")) ((\"Terry Gray\" NIL \"gray\" \"cac.washington.edu\")) ((\"Terry Gray\" NIL \"gray\" \"cac.washington.edu\")) ((NIL NIL \"imap\" \"cac.washington.edu\")) ((NIL NIL \"minutes\" \"CNRI.Reston.VA.US\") (\"John Klensin\" NIL \"KLENSIN\" \"MIT.EDU\")) NIL NIL \"<B27397-0100000@cac.washington.edu>\")";
			Envelope envelope;

			Assert.IsTrue (Envelope.TryParse (text, out envelope), "Failed to parse envelope.");

			Assert.IsTrue (envelope.Date.HasValue, "Parsed ENVELOPE date is null.");
			Assert.AreEqual ("Wed, 17 Jul 1996 02:23:25 -0700", DateUtils.FormatDate (envelope.Date.Value), "Date does not match.");
			Assert.AreEqual ("IMAP4rev1 WG mtg summary and minutes", envelope.Subject, "Subject does not match.");

			Assert.AreEqual (1, envelope.From.Count, "From counts do not match.");
			Assert.AreEqual ("\"Terry Gray\" <gray@cac.washington.edu>", envelope.From.ToString (), "From does not match.");

			Assert.AreEqual (1, envelope.Sender.Count, "Sender counts do not match.");
			Assert.AreEqual ("\"Terry Gray\" <gray@cac.washington.edu>", envelope.Sender.ToString (), "Sender does not match.");

			Assert.AreEqual (1, envelope.ReplyTo.Count, "Reply-To counts do not match.");
			Assert.AreEqual ("\"Terry Gray\" <gray@cac.washington.edu>", envelope.ReplyTo.ToString (), "Reply-To does not match.");

			Assert.AreEqual (1, envelope.To.Count, "To counts do not match.");
			Assert.AreEqual ("imap@cac.washington.edu", envelope.To.ToString (), "To does not match.");

			Assert.AreEqual (2, envelope.Cc.Count, "Cc counts do not match.");
			Assert.AreEqual ("minutes@CNRI.Reston.VA.US, \"John Klensin\" <KLENSIN@MIT.EDU>", envelope.Cc.ToString (), "Cc does not match.");

			Assert.AreEqual (0, envelope.Bcc.Count, "Bcc counts do not match.");

			Assert.IsNull (envelope.InReplyTo, "In-Reply-To is not null.");

			Assert.AreEqual ("B27397-0100000@cac.washington.edu", envelope.MessageId, "Message-Id does not match.");
		}
	}
}
