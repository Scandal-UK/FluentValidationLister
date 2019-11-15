namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;
    using FluentValidationLister.WebApiSample.Extensions;

    internal class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            this.RuleFor(a => a.Line1)
                .NotEmpty()
                .WithName("Address line 1");

            this.RuleFor(a => a.Town)
                .MaximumLength(20)
                .NotEmpty();

            this.RuleFor(a => a.County)
                .NotEmpty()
                .WithMessage("County/province must be specified.");

            this.RuleFor(a => a.Postcode)
                .NotEmpty()
                .PostcodeUk();
        }
    }
}
