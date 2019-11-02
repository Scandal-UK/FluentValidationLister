namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;

    internal class CountryValidator : AbstractValidator<Country>
    {
        public CountryValidator()
        {
            this.RuleFor(c => c.Name).NotEmpty().WithMessage("Country name must be specified (custom message).");
        }
    }
}
