// <copyright file="CountryValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

public class CountryValidator : InlineValidator<Country>
{
    public CountryValidator() => this.RuleFor(x => x.Name).NotEmpty();

    public CountryValidator(params Action<CountryValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
