namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;
    using FluentValidationLister.WebApiSample.Extensions;

    internal class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            this.RuleFor(a => a.Line1)
                .NotEmpty();

            this.RuleFor(a => a.Town)
                .MaximumLength(20)
                .NotEmpty();

            this.RuleFor(a => a.Postcode)
                .NotEmpty()
                .PostcodeUk();

            this.RuleFor(a => a.Country)
                .SetValidator(new CountryValidator());
        }
    }
}
