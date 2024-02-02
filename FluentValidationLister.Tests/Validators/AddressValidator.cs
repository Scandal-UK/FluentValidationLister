// <copyright file="AddressValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

/// <summary>
/// Validator for <see cref="Address"/> that supports rule setup at runtime.
/// </summary>
public class AddressValidator : InlineValidator<Address>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AddressValidator"/> class.
    /// </summary>
    public AddressValidator()
        : this(v => v.RuleFor(x => x.Line1).NotEmpty())
    {
        this.RuleFor(x => x.Country).SetInheritanceValidator(v =>
        {
            v.Add(new CountryValidator());
        });
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AddressValidator"/> class.
    /// </summary>
    /// <param name="actions">Initial action(s) to configure validator.</param>
    public AddressValidator(params Action<AddressValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
