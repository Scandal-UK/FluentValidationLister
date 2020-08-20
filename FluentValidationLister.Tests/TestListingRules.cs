namespace FluentValidationLister.Tests
{
    using System;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidationLister.Filter;
    using FluentValidationLister.Filter.Meta;
    using FluentValidationLister.Tests.Samples;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Xunit;

    public class TestListingRules
    {
        [Fact(DisplayName = "Null validator throws ArgumentNullException")]
        public void NullValidator_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new ValidationLister(null, typeof(Person)));

        [Fact(DisplayName = "Null modelType throws ArgumentNullException")]
        public void NullModelType_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new ValidationLister(new TestValidator(), null));

        [Fact(DisplayName = "NotEmpty() returns required rule")]
        public void NotEmpty_ReturnsRequiredRule() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty())
                .ValidatorList["Forename"]["required"].Should().Be(true);

        [Fact(DisplayName = "NotEmpty() returns correct message")]
        public void NotEmpty_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty())
                .ErrorList["Forename"]["required"].Should().Be("'Forename' must not be empty.");

        [Fact(DisplayName = "Two validators return two rules")]
        public void TwoValidators_ReturnTwoRulesAndMessages() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).NotNull().LessThan(100))
                .ValidatorList["NullableInt"].Count.Should().Be(2);

        [Fact(DisplayName = "LessThan() returns correct limit value")]
        public void LessThan_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThan(100))
                .ValidatorList["NullableInt"]["lessThan"].Should().Be(100);

        [Fact(DisplayName = "LessThan() returns correct message")]
        public void LessThan_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThan(100))
                .ErrorList["NullableInt"]["lessThan"].Should().Be("'Nullable Int' must be less than '100'.");

        [Fact(DisplayName = "LessThanOrEqualTo() returns correct limit value")]
        public void LessThanOrEqualTo_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThanOrEqualTo(100))
                .ValidatorList["NullableInt"]["lessThanOrEqualTo"].Should().Be(100);

        [Fact(DisplayName = "LessThanOrEqualTo() returns correct message")]
        public void LessThanOrEqualTo_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThanOrEqualTo(100))
                .ErrorList["NullableInt"]["lessThanOrEqualTo"].Should().Be("'Nullable Int' must be less than or equal to '100'.");

        [Fact(DisplayName = "GreaterThan() returns correct limit value")]
        public void GreaterThan_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThan(100))
                .ValidatorList["NullableInt"]["greaterThan"].Should().Be(100);

        [Fact(DisplayName = "GreaterThan() returns correct message")]
        public void GreaterThan_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThan(100))
                .ErrorList["NullableInt"]["greaterThan"].Should().Be("'Nullable Int' must be greater than '100'.");

        [Fact(DisplayName = "GreaterThanOrEqualTo() returns correct limit value")]
        public void GreaterThanOrEqualTo_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(100))
                .ValidatorList["NullableInt"]["greaterThanOrEqualTo"].Should().Be(100);

        [Fact(DisplayName = "GreaterThanOrEqualTo() returns correct message")]
        public void GreaterThanOrEqualTo_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(100))
                .ErrorList["NullableInt"]["greaterThanOrEqualTo"].Should().Be("'Nullable Int' must be greater than or equal to '100'.");

        [Fact(DisplayName = "MaxLength() returns correct limit value")]
        public void MaxLength_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).MaximumLength(10))
                .ValidatorList["Forename"]["maxLength"].Should().Be(10);

        [Fact(DisplayName = "MaxLength() returns correct message")]
        public void MaxLength_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).MaximumLength(10))
                .ErrorList["Forename"]["maxLength"].Should().Be("The length of 'Forename' must be 10 characters or fewer.");

        [Fact(DisplayName = "MinLength() returns correct limit value")]
        public void MinLength_ReturnsCorrectLimitValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).MinimumLength(10))
                .ValidatorList["Forename"]["minLength"].Should().Be(10);

        [Fact(DisplayName = "MinLength() returns correct message")]
        public void MinLength_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).MinimumLength(10))
                .ErrorList["Forename"]["minLength"].Should().Be("The length of 'Forename' must be at least 10 characters.");

        [Fact(DisplayName = "Length() returns correct message")]
        public void Length_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(2, 10))
                .ErrorList["Forename"]["length"].Should().Be("'Forename' must be between 2 and 10 characters.");

        [Fact(DisplayName = "Exact Length() returns correct value")]
        public void ExactLength_ReturnsCorrectValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(10))
                .ValidatorList["Forename"]["exactLength"].Should().Be(10);

        [Fact(DisplayName = "Exact Length() returns correct message")]
        public void ExactLength_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(10))
                .ErrorList["Forename"]["exactLength"].Should().Be("'Forename' must be 10 characters in length.");

        [Fact(DisplayName = "Equal() returns correct comparison property")]
        public void Equal_ReturnsCorrectComparisonProperty() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal(x => x.Surname))
                .ValidatorList["Forename"]["compare"].Should().Be("Surname");

        [Fact(DisplayName = "Equal() returns correct message for property")]
        public void Equal_ReturnsCorrectMessageForProperty() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal(x => x.Surname))
                .ErrorList["Forename"]["compare"].Should().Be("'Forename' must be equal to 'Surname'.");

        [Fact(DisplayName = "Equal() returns correct comparison value")]
        public void Equal_ReturnsCorrectComparisonValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal("Foo bar"))
                .ValidatorList["Forename"]["compare"].Should().Be("Foo bar");

        [Fact(DisplayName = "Equal() returns correct message for value")]
        public void Equal_ReturnsCorrectMessageForValue() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal("Foo bar"))
                .ErrorList["Forename"]["compare"].Should().Be("'Forename' must be equal to 'Foo bar'.");

        [Fact(DisplayName = "Matches() returns regular expression")]
        public void Matches_ReturnsRegularExpression() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Matches("^[A-Za-z]+$"))
                .ValidatorList["Forename"]["regex"].Should().Be("^[A-Za-z]+$");

        [Fact(DisplayName = "Matches() returns correct message")]
        public void Matches_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Matches("^[A-Za-z]+$"))
                .ErrorList["Forename"]["regex"].Should().Be("'Forename' is not in the correct format.");

        [Fact(DisplayName = "EmailAddress() returns regular expression")]
        public void EmailAddress_ReturnsRegularExpression() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Email).EmailAddress())
                .ValidatorList["Email"]["regex"].Should().NotBeNull();

        [Fact(DisplayName = "EmailAddress() returns correct message")]
        public void EmailAddress_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Email).EmailAddress())
                .ErrorList["Email"]["regex"].Should().Be("'Email' is not a valid email address.");

        [Fact(DisplayName = "InclusiveBetween() returns correct message")]
        public void InclusiveBetween_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).InclusiveBetween(5, 8))
                .ErrorList["AnotherInt"]["range"].Should().Be("'Another Int' must be between 5 and 8.");

        [Fact(DisplayName = "ExclusiveBetween() returns correct message")]
        public void ExclusiveBetween_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).ExclusiveBetween(5, 8))
                .ErrorList["AnotherInt"]["exclusiveRange"].Should().Be("'Another Int' must be between 5 and 8 (exclusive).");

        [Fact(DisplayName = "Validator includes child validator rules")]
        public void Validator_IncludesChildValidatorRules() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
                .ValidatorList["Address.Line1"].Should().NotBeNull();

        [Fact(DisplayName = "Child validator returns correct message")]
        public void ChildValidator_ReturnsCorrectMessage() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
                .ErrorList["Address.Line1"]["required"].Should().Be("'Line1' must not be empty.");

        [Fact(DisplayName = "Validator includes grandchild validator rules")]
        public void Validator_IncludesGrandChildValidatorRules() =>
            this.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
                .ValidatorList["Address.Country.Name"].Should().NotBeNull();

        [Fact(DisplayName = "Validator includes collection rules")]
        public void Validator_IncludesCollectionRules() =>
            this.GetValidatorRules(v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator()))
                .ValidatorList["Orders.Amount"].Should().NotBeNull();

        [Fact(DisplayName = "Types include string")]
        public void Types_IncludeString() =>
            this.GetValidatorRules().TypeList["Surname"].Should().Be("string");

        [Fact(DisplayName = "Types include number")]
        public void Types_IncludeNumber() =>
            this.GetValidatorRules().TypeList["Id"].Should().Be("number");

        [Fact(DisplayName = "Types include boolean")]
        public void Types_IncludeBoolean() =>
            this.GetValidatorRules().TypeList["IsActive"].Should().Be("boolean");

        [Fact(DisplayName = "Types include date")]
        public void Types_IncludeDate() =>
            this.GetValidatorRules().TypeList["DateOfBirth"].Should().Be("date");

        [Fact(DisplayName = "Types include child types")]
        public void Types_IncludeChildTypes() =>
            this.GetValidatorRules().TypeList["Address.Line1"].Should().Be("string");

        [Fact(DisplayName = "Types include collection types")]
        public void Types_IncludeCollectionTypes() =>
            this.GetValidatorRules().TypeList["Orders.Amount"].Should().Be("number");

        [Fact(DisplayName = "When serialised returns correct message")]
        public void WhenSerialised_ReturnsCorrectMessage() =>
            this.GetDeserialisedValidatorRules(this.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty()))["errorList"]["forename"]["required"].ToString().Should().Be("'Forename' must not be empty.");

        [Fact(DisplayName = "Length() when serialised, returns correct values")]
        public void Length_WhenSerialisedReturnsCorrectValues()
        {
            var forenameRules = this.GetDeserialisedValidatorRules(this.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(2, 10)))["validatorList"]["forename"]["length"];
            forenameRules["min"].ToObject<int>().Should().Be(2);
            forenameRules["max"].ToObject<int>().Should().Be(10);
        }

        [Fact(DisplayName = "InclusiveBetween() when serialised, returns correct values")]
        public void InclusiveBetween_WhenSerialisedReturnsCorrectValues()
        {
            var anotherIntRules = this.GetDeserialisedValidatorRules(this.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).InclusiveBetween(2, 10)))["validatorList"]["anotherInt"]["range"];
            anotherIntRules["from"].ToObject<int>().Should().Be(2);
            anotherIntRules["to"].ToObject<int>().Should().Be(10);
        }

        [Fact(DisplayName = "ExclusiveBetween() returns correct values")]
        public void ExclusiveBetween_ReturnsCorrectValues()
        {
            var anotherIntRules = this.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt)
                .ExclusiveBetween(5, 8))
                .ValidatorList["AnotherInt"];

            anotherIntRules["exclusiveRange"].Should().BeOfType(typeof(FromToValues));

            FromToValues values = (FromToValues)anotherIntRules["exclusiveRange"];
            values.From.Should().Be(5);
            values.To.Should().Be(8);
        }

        // Helper functions

        private ValidatorRules GetValidatorRules(params Action<TestValidator>[] actions) =>
            new ValidationLister(new TestValidator(actions), typeof(Person)).GetValidatorRules();

        private JObject GetDeserialisedValidatorRules(ValidatorRules rules) =>
            JObject.Parse(JsonConvert.SerializeObject(
                rules,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
}
