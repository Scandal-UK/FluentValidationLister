namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;

    internal class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            this.RuleFor(a => a.Line1).NotEmpty();

            this.RuleFor(a => a.Town).NotEmpty();

            this.RuleFor(a => a.Postcode)
                .NotEmpty()
                .MaximumLength(10);

            this.RuleFor(a => a.Country)
                .NotNull()
                .SetValidator(new CountryValidator());
        }
    }
}
