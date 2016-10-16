using FluentValidation;
using Model;

namespace Service.Validator
{
    public class EmailValidator: AbstractValidator<EmailAddress>
    {
        public EmailValidator()
        {
            RuleFor(ea => ea.Email).NotEmpty();

            RuleFor(ea => ea.Email).NotNull();

            RuleFor(ea => ea.Email).EmailAddress();
        }
    }
}
