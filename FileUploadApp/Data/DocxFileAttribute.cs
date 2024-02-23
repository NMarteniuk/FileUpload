using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Packaging;
namespace FileUploadApp.Data
{
    public class DocxFileAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IBrowserFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.Name);
                if (extension.ToLower() != ".docx")
                {
                    return new ValidationResult("Аile must be in .docx format");
                }
            }
            return ValidationResult.Success;
        }
    }
}
