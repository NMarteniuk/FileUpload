using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Azure.WebJobs.Extensions.SendGrid;
using Azure.Storage.Blobs;
using Azure;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;



namespace FileUploadFunction
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("files/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "files");
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            BlobProperties blobProperties = blobClient.GetProperties();
            string email = blobProperties.Metadata["email"];

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var message = new SendGridMessage()
            {
                From = new EmailAddress("nazar_marteniuk1407@tntu.edu.ua", "Nazar"),
                Subject = "Blob",
                PlainTextContent = $"Your blob with name {name} has been processed.",
            };
            message.AddTo(new EmailAddress(email));
            client.SendEmailAsync(message);
            log.LogInformation($"Email sent to {email}");
        }

    }
}

