// <copyright file="TestValidator.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Validators;

using System;
using FluentValidation;
using FluentValidationLister.Tests.Models;

/// <summary>
/// Validator for <see cref="Person"/> that supports rule setup at runtime.
/// </summary>
/// <remarks><see href="https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation.Tests/TestValidator.cs">Original source.</see></remarks>
public class TestValidator : InlineValidator<Person>
{
    /// <summary>
    /// Initialises a new instance of the <see cref="TestValidator"/> class.
    /// </summary>
    /// <param name="actions">Initial action(s) to configure validator.</param>
    public TestValidator(params Action<TestValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}
