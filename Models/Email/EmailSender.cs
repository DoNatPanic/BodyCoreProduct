using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace BodyCore.Models.Email
{
	public class EmailSender 
	{
		public async Task SendEmailAsync( Message message )
		{
			var mailMessage = CreateEmailMessage(message);

			await SendAsync(mailMessage);
		}

		public MimeMessage CreateEmailMessage( Message message )
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress("HealthyWeight.ru", "weightdecreaseline@gmail.com"));
			emailMessage.To.AddRange(message.To);
			emailMessage.Subject = message.Subject;

			var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("{0}", message.Content)};

			if ( message.Attachment != null )
			{
				byte[] fileBytes = message.Attachment.FileContents;

				bodyBuilder.Attachments.Add(message.FileName, fileBytes, ContentType.Parse(message.Attachment.ContentType));

			}
			emailMessage.Body = bodyBuilder.ToMessageBody();
			return emailMessage;
		}

		private async Task SendAsync(MimeMessage mailMessage)
		{
		    using (var client = new SmtpClient())
		    {
		        try
		        {
		            await client.ConnectAsync("smtp.gmail.com", 465, true);
					await client.AuthenticateAsync("weightdecreaseline@gmail.com", "ElewBEM6");

					await client.SendAsync(mailMessage);
		        }
		        catch
		        {
		            //log an error message or throw an exception, or both.
		            throw;
		        }
		        finally
		        {
		            await client.DisconnectAsync(true);
		            client.Dispose();
				}
			}
		}
	}
}
