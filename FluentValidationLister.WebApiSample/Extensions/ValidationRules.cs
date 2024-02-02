// <copyright file="ValidationRules.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Extensions;

using FluentValidation;

/// <summary>
/// A class to extend the <see cref="IRuleBuilder{T, TProperty}"/> with extra custom properties.
/// </summary>
public static class ValidationRules
{
    /// <summary>
    /// Validate UK postal code format.
    /// </summary>
    /// <typeparam name="T">Entity for validation.</typeparam>
    /// <param name="ruleBuilder">Instance of <see cref="IRuleBuilder{T, TProperty}"/>.</param>
    /// <returns>Instance of <see cref="IRuleBuilderOptions{T, TProperty}"/>.</returns>
    /// <remarks>Source: <see href="https://stackoverflow.com/questions/164979/uk-postcode-regex-comprehensive#17507615"/>.</remarks>
    public static IRuleBuilderOptions<T, string> PostcodeUk<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Matches(@"^(GIR ?0AA|[A-PR-UWYZ]([0-9]{1,2}|([A-HK-Y][0-9]([0-9ABEHMNPRV-Y])?)|[0-9][A-HJKPS-UW]) ?[0-9][ABD-HJLNP-UW-Z]{2})$")
            .WithMessage("Please use a valid UK postcode.");
}
