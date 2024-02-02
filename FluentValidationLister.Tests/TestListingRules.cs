// <copyright file="TestListingRules.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests;

using System;
using FluentAssertions;
using FluentValidation;
using FluentValidationLister.Filter;
using FluentValidationLister.Filter.Meta;
using FluentValidationLister.Tests.Models;
using FluentValidationLister.Tests.Validators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class TestListingRules
{
    [Fact(DisplayName = "Null validator throws ArgumentNullException")]
    public void NullValidator_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationLister(validator: null, typeof(Person), new ServiceCollection().BuildServiceProvider()));

    [Fact(DisplayName = "Null modelType throws ArgumentNullException")]
    public void NullModelType_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationLister(new TestValidator(), modelType: null, new ServiceCollection().BuildServiceProvider()));

    [Fact(DisplayName = "Null serviceProvider throws ArgumentNullException")]
    public void NullServiceProvider_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationLister(new TestValidator(), typeof(Person), serviceProvider: null));

    [Fact(DisplayName = "NotEmpty() returns required rule")]
    public void NotEmpty_ReturnsRequiredRule() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty())
            .ValidatorList[key: nameof(Person.Forename)]["required"].Should().Be(true);

    [Fact(DisplayName = "Two validators return two rules")]
    public void TwoValidators_ReturnTwoRulesAndMessages() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).NotNull().LessThan(100))
            .ValidatorList[key: nameof(Person.NullableInt)].Count.Should().Be(2);

    [Fact(DisplayName = "LessThan() returns correct limit value")]
    public void LessThan_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThan(100))
            .ValidatorList[key: nameof(Person.NullableInt)]["lessThan"].Should().Be(100);

    [Fact(DisplayName = "LessThanOrEqualTo() returns correct limit value")]
    public void LessThanOrEqualTo_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThanOrEqualTo(100))
            .ValidatorList[key: nameof(Person.NullableInt)]["lessThanOrEqualTo"].Should().Be(100);

    [Fact(DisplayName = "GreaterThan() returns correct limit value")]
    public void GreaterThan_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThan(100))
            .ValidatorList[key: nameof(Person.NullableInt)]["greaterThan"].Should().Be(100);

    [Fact(DisplayName = "GreaterThanOrEqualTo() returns correct limit value")]
    public void GreaterThanOrEqualTo_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(100))
            .ValidatorList[key: nameof(Person.NullableInt)]["greaterThanOrEqualTo"].Should().Be(100);

    [Fact(DisplayName = "MaxLength() returns correct limit value")]
    public void MaxLength_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MaximumLength(10))
            .ValidatorList[key: nameof(Person.Forename)]["maxLength"].Should().Be(10);

    [Fact(DisplayName = "MinLength() returns correct limit value")]
    public void MinLength_ReturnsCorrectLimitValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MinimumLength(10))
            .ValidatorList[key: nameof(Person.Forename)]["minLength"].Should().Be(10);

    [Fact(DisplayName = "Exact Length() returns correct value")]
    public void ExactLength_ReturnsCorrectValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(10))
            .ValidatorList[key: nameof(Person.Forename)]["exactLength"].Should().Be(10);

    [Fact(DisplayName = "Equal() returns correct comparison property")]
    public void Equal_ReturnsCorrectComparisonProperty() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal(x => x.Surname))
            .ValidatorList[key: nameof(Person.Forename)]["compare"].Should().Be("Surname");

    [Fact(DisplayName = "Equal() returns correct comparison value")]
    public void Equal_ReturnsCorrectComparisonValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal("Foo bar"))
            .ValidatorList[key: nameof(Person.Forename)]["compare"].Should().Be("Foo bar");

    [Fact(DisplayName = "Matches() returns regular expression")]
    public void Matches_ReturnsRegularExpression() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Matches("^[A-Za-z]+$"))
            .ValidatorList[key: nameof(Person.Forename)]["regex"].Should().Be("^[A-Za-z]+$");

    [Fact(DisplayName = "EmailAddress() returns regular expression")]
    public void EmailAddress_ReturnsRegularExpression() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Email).EmailAddress())
            .ValidatorList[key: nameof(Person.Email)]["regex"].Should().NotBeNull();

    [Fact(DisplayName = "Validator includes child validator rules")]
    public void Validator_IncludesChildValidatorRules() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
            .ValidatorList["Address.Line1"].Should().NotBeNull();

    [Fact(DisplayName = "Validator includes grandchild validator rules")]
    public void Validator_IncludesGrandChildValidatorRules() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
            .ValidatorList["Address.Country.Name"].Should().NotBeNull();

    [Fact(DisplayName = "Validator includes collection rules")]
    public void Validator_IncludesCollectionRules() =>
        TestListingHelper.GetValidatorRules(v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator()))
            .ValidatorList["Orders.Amount"].Should().NotBeNull();

    [Fact(DisplayName = "When serialised returns correct message")]
    public void WhenSerialised_ReturnsCorrectMessage() =>
        TestListingHelper.GetDeserialisedValidatorRules(TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty()))["errorList"]["forename"]["required"].ToString().Should().Be("'Forename' must not be empty.");

    [Fact(DisplayName = "Length() when serialised, returns correct values")]
    public void Length_WhenSerialisedReturnsCorrectValues()
    {
        var forenameRules = TestListingHelper.GetDeserialisedValidatorRules(TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(2, 10)))["validatorList"]["forename"]["length"];
        forenameRules["min"].ToObject<int>().Should().Be(2);
        forenameRules["max"].ToObject<int>().Should().Be(10);
    }

    [Fact(DisplayName = "InclusiveBetween() when serialised, returns correct values")]
    public void InclusiveBetween_WhenSerialisedReturnsCorrectValues()
    {
        var anotherIntRules = TestListingHelper.GetDeserialisedValidatorRules(TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).InclusiveBetween(2, 10)))["validatorList"]["anotherInt"]["range"];
        anotherIntRules["from"].ToObject<int>().Should().Be(2);
        anotherIntRules["to"].ToObject<int>().Should().Be(10);
    }

    [Fact(DisplayName = "ExclusiveBetween() returns correct values")]
    public void ExclusiveBetween_ReturnsCorrectValues()
    {
        var anotherIntRules = TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt)
            .ExclusiveBetween(5, 8))
            .ValidatorList[nameof(Person.AnotherInt)];

        anotherIntRules["exclusiveRange"].Should().BeOfType(typeof(FromToValues));

        FromToValues values = (FromToValues)anotherIntRules["exclusiveRange"];
        values.From.Should().Be(5);
        values.To.Should().Be(8);
    }
}
