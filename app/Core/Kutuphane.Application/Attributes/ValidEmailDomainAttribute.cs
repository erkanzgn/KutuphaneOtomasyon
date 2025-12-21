using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Kutuphane.Application.Attributes 
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

         
            if (string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success;
            }

            var parts = email.Split('@');
            if (parts.Length != 2)
            {
              
                return new ValidationResult("Geçersiz e-posta formatı.");
            }

            var domain = parts[1];

            try
            {
                
                var hostEntry = Dns.GetHostEntry(domain);

                if (hostEntry.AddressList.Length > 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"'{domain}' adresi geçerli bir e-posta sunucusu barındırmıyor.");
                }
            }
            catch (Exception)
            {
                return new ValidationResult($"'{domain}' geçerli bir alan adı değildir. Lütfen kontrol ediniz.");
            }
        }
    }
}