namespace FluentValidationLister.Tests
{
    using FluentAssertions;
    using FluentValidation;
    using FluentValidationLister.Tests.Samples;
    using Xunit;

    public class TestListingMessages
    {
        [Fact(DisplayName = "NotEmpty() returns correct message")]
        public void NotEmpty_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty())
                .ErrorList["Forename"]["required"].Should().Be("'Forename' must not be empty.");

        [Fact(DisplayName = "LessThan() returns correct message")]
        public void LessThan_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThan(100))
                .ErrorList["NullableInt"]["lessThan"].Should().Be("'Nullable Int' must be less than '100'.");

        [Fact(DisplayName = "LessThanOrEqualTo() returns correct message")]
        public void LessThanOrEqualTo_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThanOrEqualTo(100))
                .ErrorList["NullableInt"]["lessThanOrEqualTo"].Should().Be("'Nullable Int' must be less than or equal to '100'.");

        [Fact(DisplayName = "GreaterThan() returns correct message")]
        public void GreaterThan_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThan(100))
                .ErrorList["NullableInt"]["greaterThan"].Should().Be("'Nullable Int' must be greater than '100'.");

        [Fact(DisplayName = "GreaterThanOrEqualTo() returns correct message")]
        public void GreaterThanOrEqualTo_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(100))
                .ErrorList["NullableInt"]["greaterThanOrEqualTo"].Should().Be("'Nullable Int' must be greater than or equal to '100'.");

        [Fact(DisplayName = "MaxLength() returns correct message")]
        public void MaxLength_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MaximumLength(10))
                .ErrorList["Forename"]["maxLength"].Should().Be("The length of 'Forename' must be 10 characters or fewer.");

        [Fact(DisplayName = "MinLength() returns correct message")]
        public void MinLength_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MinimumLength(10))
                .ErrorList["Forename"]["minLength"].Should().Be("The length of 'Forename' must be at least 10 characters.");

        [Fact(DisplayName = "Length() returns correct message")]
        public void Length_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(2, 10))
                .ErrorList["Forename"]["length"].Should().Be("'Forename' must be between 2 and 10 characters.");

        [Fact(DisplayName = "Exact Length() returns correct message")]
        public void ExactLength_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(10))
                .ErrorList["Forename"]["exactLength"].Should().Be("'Forename' must be 10 characters in length.");

        [Fact(DisplayName = "Equal() returns correct message for property")]
        public void Equal_ReturnsCorrectMessageForProperty() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal(x => x.Surname))
                .ErrorList["Forename"]["compare"].Should().Be("'Forename' must be equal to 'Surname'.");

        [Fact(DisplayName = "Equal() returns correct message for value")]
        public void Equal_ReturnsCorrectMessageForValue() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal("Foo bar"))
                .ErrorList["Forename"]["compare"].Should().Be("'Forename' must be equal to 'Foo bar'.");

        [Fact(DisplayName = "Matches() returns correct message")]
        public void Matches_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Matches("^[A-Za-z]+$"))
                .ErrorList["Forename"]["regex"].Should().Be("'Forename' is not in the correct format.");

        [Fact(DisplayName = "EmailAddress() returns correct message")]
        public void EmailAddress_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Email).EmailAddress())
                .ErrorList["Email"]["regex"].Should().Be("'Email' is not a valid email address.");

        [Fact(DisplayName = "InclusiveBetween() returns correct message")]
        public void InclusiveBetween_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).InclusiveBetween(5, 8))
                .ErrorList["AnotherInt"]["range"].Should().Be("'Another Int' must be between 5 and 8.");

        [Fact(DisplayName = "ExclusiveBetween() returns correct message")]
        public void ExclusiveBetween_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).ExclusiveBetween(5, 8))
                .ErrorList["AnotherInt"]["exclusiveRange"].Should().Be("'Another Int' must be between 5 and 8 (exclusive).");

        [Fact(DisplayName = "Child validator returns correct message")]
        public void ChildValidator_ReturnsCorrectMessage() =>
            TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
                .ErrorList["Address.Line1"]["required"].Should().Be("'Line1' must not be empty.");
    }
}
