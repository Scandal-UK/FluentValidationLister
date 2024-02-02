// <copyright file="OrderValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

/// <summary>
/// Validator for <see cref="Order"/> that supports rule setup at runtime.
/// </summary>
public class OrderValidator : InlineValidator<Order>
{
    /// <summary> Initialises a new instance of the <see cref="OrderValidator"/> class. </summary>
    public OrderValidator()
        : this(v => v.RuleFor(x => x.Amount).NotNull())
    {
    }

    /// <summary> Initialises a new instance of the <see cref="OrderValidator"/> class. </summary>
    /// <param name="actions">Initial action(s) to configure validator.</param>
    public OrderValidator(params Action<OrderValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
