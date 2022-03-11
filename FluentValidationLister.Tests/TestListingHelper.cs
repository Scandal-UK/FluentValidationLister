// <copyright file="TestListingHelper.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests
{
    using System;
    using FluentValidation;
    using FluentValidationLister.Filter;
    using FluentValidationLister.Filter.Meta;
    using FluentValidationLister.Tests.Samples;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    public static class TestListingHelper
    {
        public static ValidatorRules GetValidatorRules(params Action<TestValidator>[] actions) =>
            new ValidationLister(new TestValidator(actions), typeof(Person), GetServiceProvider()).GetValidatorRules();

        public static IServiceProvider GetServiceProvider() =>
            new ServiceCollection()
            .AddTransient<IValidator<Address>, AddressValidator>()
            .AddTransient<IValidator<Order>, OrderValidator>()
            .AddTransient<IValidator<Country>, CountryValidator>()
            .BuildServiceProvider();

        public static JObject GetDeserialisedValidatorRules(ValidatorRules rules) =>
            JObject.Parse(JsonConvert.SerializeObject(
                rules,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
}
