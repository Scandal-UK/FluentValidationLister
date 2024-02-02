// <copyright file="CountryValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

/// <summary>
/// Validator for <see cref="Country"/> that supports rule setup at runtime.
/// </summary>
public class CountryValidator : InlineValidator<Country>
{
    /// <summary> Initialises a new instance of the <see cref="CountryValidator"/> class. </summary>
    public CountryValidator() => this.RuleFor(x => x.Name).NotEmpty();

    /// <summary> Initialises a new instance of the <see cref="CountryValidator"/> class. </summary>
    /// <param name="actions">Initial action(s) to configure validator.</param>
    public CountryValidator(params Action<CountryValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
