namespace FluentValidationLister.Tests
{
    using FluentAssertions;
    using Xunit;

    public class TestListingTypes
    {
        [Fact(DisplayName = "Types include string")]
        public void Types_IncludeString() =>
            TestListingHelper.GetValidatorRules().TypeList["Surname"].Should().Be("string");

        [Fact(DisplayName = "Types include number")]
        public void Types_IncludeNumber() =>
            TestListingHelper.GetValidatorRules().TypeList["Id"].Should().Be("number");

        [Fact(DisplayName = "Types include boolean")]
        public void Types_IncludeBoolean() =>
            TestListingHelper.GetValidatorRules().TypeList["IsActive"].Should().Be("boolean");

        [Fact(DisplayName = "Types include date")]
        public void Types_IncludeDate() =>
            TestListingHelper.GetValidatorRules().TypeList["DateOfBirth"].Should().Be("date");

        [Fact(DisplayName = "Types include child types")]
        public void Types_IncludeChildTypes() =>
            TestListingHelper.GetValidatorRules().TypeList["Address.Line1"].Should().Be("string");

        [Fact(DisplayName = "Types include collection types")]
        public void Types_IncludeCollectionTypes() =>
            TestListingHelper.GetValidatorRules().TypeList["Orders.Amount"].Should().Be("number");
    }
}
