namespace FluentValidationLister.WebApiSample.Models.Validators
{
    using FluentValidation;
    using FluentValidation.Validators;

    internal class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            this.RuleFor(p => p.Surname)
                .NotEmpty();

            this.RuleFor(p => p.Forename)
                .NotEmpty();

            this.RuleFor(p => p.Age)
                .NotNull()
                .InclusiveBetween(16, 60);

            this.RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

            this.RuleFor(p => p.Address)
                .NotNull()
                .WithMessage($"'{nameof(Address)}' is required.")
                .SetValidator(new AddressValidator());
        }
    }
}
