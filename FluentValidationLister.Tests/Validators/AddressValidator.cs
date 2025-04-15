// <copyright file="AddressValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

public class AddressValidator : InlineValidator<Address>
{
    public AddressValidator()
        : this(v => v.RuleFor(x => x.Line1).NotEmpty())
    {
        this.RuleFor(x => x.Country).SetInheritanceValidator(v =>
        {
            v.Add(new CountryValidator());
        });
    }

    public AddressValidator(params Action<AddressValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
