namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;
    using FluentValidationLister.WebApiSample.Extensions;

    /// <summary>Validator for the <see cref="Address"/> class.</summary>
    internal class AddressValidator : AbstractValidator<Address>
    {
        /// <summary>Initialises a new instance of the <see cref="AddressValidator"/> class.</summary>
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
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .PostcodeUk();
        }
    }
}
