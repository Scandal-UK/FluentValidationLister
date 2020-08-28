namespace FluentValidationLister.Tests
{
    using FluentAssertions;
    using FluentValidationLister.Tests.Samples;
    using Xunit;

    public class TestListingTypes
    {
        [Fact(DisplayName = "Types include string")]
        public void Types_IncludeString() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Surname)]
            .Should().Be("string");

        [Fact(DisplayName = "Types include number")]
        public void Types_IncludeNumber() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Id)]
            .Should().Be("number");

        [Fact(DisplayName = "Types include boolean")]
        public void Types_IncludeBoolean() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.IsActive)]
            .Should().Be("boolean");

        [Fact(DisplayName = "Types include date")]
        public void Types_IncludeDate() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.DateOfBirth)]
            .Should().Be("date");

        [Fact(DisplayName = "Types include child types")]
        public void Types_IncludeChildTypes() =>
            TestListingHelper.GetValidatorRules().TypeList[$"{nameof(Address)}.{nameof(Address.Line1)}"]
            .Should().Be("string");

        [Fact(DisplayName = "Types include collection types")]
        public void Types_IncludeCollectionTypes() =>
            TestListingHelper.GetValidatorRules().TypeList[$"{nameof(Person.Orders)}.{nameof(Order.Amount)}"]
            .Should().Be("number");

        [Fact(DisplayName = "Types; Integer? returns number")]
        public void Types_IntegerReturnsNumber() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.NullableInt)]
            .Should().Be("number");

        [Fact(DisplayName = "Types; Decimal? returns number")]
        public void Types_DecimalReturnsNumber() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Discount)]
            .Should().Be("number");

        [Fact(DisplayName = "Types; Double? returns number")]
        public void Types_DoubleReturnsNumber() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Age)]
            .Should().Be("number");

        [Fact(DisplayName = "Types; Float? returns number")]
        public void Types_FloatOrSingleReturnsNumber() =>
            TestListingHelper.GetValidatorRules().TypeList[nameof(Person.SomeSingleValue)]
            .Should().Be("number");
    }
}
