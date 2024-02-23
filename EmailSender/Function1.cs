using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailSender
{
    public class Function1
    {
        [FunctionName("FunctionSender")]
        public async Task Run([BlobTrigger("files/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, Binder binder, ILogger log)
        {
            var blobAttribute = new BlobAttribute($"files/{name}");
            var cloudBlockBlob = await binder.BindAsync<CloudBlockBlob>(blobAttribute);
            await cloudBlockBlob.FetchAttributesAsync();
            string email = cloudBlockBlob.Metadata["email"];
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("nazar_marteniuk1407@tntu.edu.ua", "Nazar"),
                Subject = "Blob",
                PlainTextContent = $"Your blob with name {name} has been processed.",
      
            };
            msg.AddTo(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);
            log.LogInformation($"Email sent to {email}");
        }
    }
}

