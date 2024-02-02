// <copyright file="PersonValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Models.Validators;

using FluentValidation;
using FluentValidation.Validators;

/// <summary>Validator for the <see cref="Person"/> class.</summary>
internal class PersonValidator : AbstractValidator<Person>
{
    /// <summary>Initialises a new instance of the <see cref="PersonValidator"/> class.</summary>
    public PersonValidator()
    {
        this.RuleFor(p => p.Surname)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(3, 20);

        this.RuleFor(p => p.Forename)
            .NotEmpty();

        this.RuleFor(p => p.Age)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .InclusiveBetween(16, 60);

        this.RuleFor(p => p.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

        this.RuleFor(p => p.SaleOfSoulAgreed)
            .NotNull();

        this.RuleFor(p => p.Address)
            .NotNull()
            .WithMessage($"'{nameof(Address)}' is required.")
            .SetValidator(new AddressValidator());
    }
}
