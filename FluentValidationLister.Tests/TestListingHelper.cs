namespace FluentValidationLister.Tests
{
    using System;
    using FluentValidationLister.Filter;
    using FluentValidationLister.Filter.Meta;
    using FluentValidationLister.Tests.Samples;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    public static class TestListingHelper
    {
        public static ValidatorRules GetValidatorRules(params Action<TestValidator>[] actions) =>
            new ValidationLister(new TestValidator(actions), typeof(Person)).GetValidatorRules();

        public static JObject GetDeserialisedValidatorRules(ValidatorRules rules) =>
            JObject.Parse(JsonConvert.SerializeObject(
                rules,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
}
