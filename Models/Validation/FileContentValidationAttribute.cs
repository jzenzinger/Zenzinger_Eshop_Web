using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Zenzinger_Eshop_Web.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class FileContentValidationAttribute : ValidationAttribute
    {
        public string ContentType { get; set; }

        public FileContentValidationAttribute(string contentType)
        {
            ContentType = contentType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (formFile.ContentType.ToLower().Contains(this.ContentType.ToLower()))
                {
                    return  ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"Content type of the file ({validationContext.MemberName}) is not {ContentType}.");
                }
            }

            throw new NotImplementedException($"{nameof(FileContentValidationAttribute)} attribute is not implemented for this type of object: {value.GetType()}");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-filecontent", $"Content oof the file is not {ContentType}.");
            context.Attributes.Add("data-val-filecontent-type", ContentType.ToLower());
        }
    }
}