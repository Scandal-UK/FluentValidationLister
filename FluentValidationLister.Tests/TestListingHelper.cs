// <copyright file="TestListingHelper.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests;

using System;
using FluentValidation;
using FluentValidationLister.Filter;
using FluentValidationLister.Filter.Meta;
using FluentValidationLister.Tests.Models;
using FluentValidationLister.Tests.Validators;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

/// <summary> Class to provide helper methods to test a listing. </summary>
public static class TestListingHelper
{
    /// <summary> Gets validator rules based on provided actions. </summary>
    /// <param name="actions">Method determining which rule to get.</param>
    /// <returns>Instance of <see cref="ValidatorRules"/>.</returns>
    public static ValidatorRules GetValidatorRules(params Action<TestValidator>[] actions) =>
        new ValidationLister(new TestValidator(actions), typeof(Person), GetServiceProvider()).GetValidatorRules();

    /// <summary> Gets the container with validators ready configured. </summary>
    /// <returns>Instance of <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider GetServiceProvider() =>
        new ServiceCollection()
        .AddTransient<IValidator<Address>, AddressValidator>()
        .AddTransient<IValidator<Order>, OrderValidator>()
        .AddTransient<IValidator<Country>, CountryValidator>()
        .BuildServiceProvider();

    /// <summary> Gets the validator rules deserialised to JSON objects. </summary>
    /// <param name="rules">Instance of <see cref="ValidatorRules"/>.</param>
    /// <returns>Instance of <see cref="JObject"/>.</returns>
    public static JObject GetDeserialisedValidatorRules(ValidatorRules rules) =>
        JObject.Parse(JsonConvert.SerializeObject(
            rules,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
}
