// <copyright file="OrderValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

public class OrderValidator : InlineValidator<Order>
{
    public OrderValidator()
        : this(v => v.RuleFor(x => x.Amount).NotNull())
    {
    }

    public OrderValidator(params Action<OrderValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
