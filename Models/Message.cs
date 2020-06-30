using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BodyCore.Models
{
	public class Message
	{
		public List<MailboxAddress> To { get; set; }
		public string Subject { get; set; }
		public string Content { get; set; }
		public string FileName { get; set; }

		public FileContentResult Attachment { get; set; }

		public Message( IEnumerable<string> to, string subject, string content, FileContentResult attachment )
		{
			To = new List<MailboxAddress>();

			To.AddRange(to.Select(x => new MailboxAddress(x)));
			Subject = subject;
			Content = content;
			Attachment = attachment;
			if (attachment != null )
			{
				FileName = attachment.FileDownloadName;
			}
			
		}
	}
}
