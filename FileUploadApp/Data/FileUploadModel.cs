using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace FileUploadApp.Data
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "File is necessary")]
        [DocxFile]
        public IBrowserFile File { get; set; }

        [Required(ErrorMessage = "Email is necessary")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
