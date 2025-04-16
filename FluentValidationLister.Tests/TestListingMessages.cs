// <copyright file="TestListingMessages.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests;

using FluentAssertions;
using FluentValidation;
using FluentValidationLister.Tests.Models;
using FluentValidationLister.Tests.Validators;
using Xunit;

public class TestListingMessages
{
    [Fact(DisplayName = "NotEmpty() returns correct message")]
    public void NotEmpty_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).NotEmpty())
            .ErrorList[nameof(Person.Forename)]["required"].Should().Be("'Forename' must not be empty.");

    [Fact(DisplayName = "LessThan() returns correct message")]
    public void LessThan_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThan(100))
            .ErrorList[nameof(Person.NullableInt)]["lessThan"].Should().Be("'Nullable Int' must be less than '100'.");

    [Fact(DisplayName = "LessThanOrEqualTo() returns correct message")]
    public void LessThanOrEqualTo_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).LessThanOrEqualTo(100))
            .ErrorList[nameof(Person.NullableInt)]["lessThanOrEqualTo"].Should().Be("'Nullable Int' must be less than or equal to '100'.");

    [Fact(DisplayName = "GreaterThan() returns correct message")]
    public void GreaterThan_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThan(100))
            .ErrorList[nameof(Person.NullableInt)]["greaterThan"].Should().Be("'Nullable Int' must be greater than '100'.");

    [Fact(DisplayName = "GreaterThanOrEqualTo() returns correct message")]
    public void GreaterThanOrEqualTo_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(100))
            .ErrorList[nameof(Person.NullableInt)]["greaterThanOrEqualTo"].Should().Be("'Nullable Int' must be greater than or equal to '100'.");

    [Fact(DisplayName = "MaxLength() returns correct message")]
    public void MaxLength_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MaximumLength(10))
            .ErrorList[nameof(Person.Forename)]["maxLength"].Should().Be("The length of 'Forename' must be 10 characters or fewer.");

    [Fact(DisplayName = "MinLength() returns correct message")]
    public void MinLength_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).MinimumLength(10))
            .ErrorList[nameof(Person.Forename)]["minLength"].Should().Be("The length of 'Forename' must be at least 10 characters.");

    [Fact(DisplayName = "Length() returns correct message")]
    public void Length_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(2, 10))
            .ErrorList[nameof(Person.Forename)]["length"].Should().Be("'Forename' must be between 2 and 10 characters.");

    [Fact(DisplayName = "ExactLength() returns correct message")]
    public void ExactLength_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Length(10))
            .ErrorList[nameof(Person.Forename)]["exactLength"].Should().Be("'Forename' must be 10 characters in length.");

    [Fact(DisplayName = "Equal() returns correct message for property")]
    public void Equal_ReturnsCorrectMessageForProperty() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal(x => x.Surname))
            .ErrorList[nameof(Person.Forename)]["compare"].Should().Be("'Forename' must be equal to 'Surname'.");

    [Fact(DisplayName = "Equal() returns correct message for value")]
    public void Equal_ReturnsCorrectMessageForValue() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Equal("Foo bar"))
            .ErrorList[nameof(Person.Forename)]["compare"].Should().Be("'Forename' must be equal to 'Foo bar'.");

    [Fact(DisplayName = "Matches() returns correct message")]
    public void Matches_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Forename).Matches("^[A-Za-z]+$"))
            .ErrorList[nameof(Person.Forename)]["regex"].Should().Be("'Forename' is not in the correct format.");

    [Fact(DisplayName = "EmailAddress() returns correct message")]
    public void EmailAddress_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Email).EmailAddress())
            .ErrorList[nameof(Person.Email)]["regex"].Should().Be("'Email' is not a valid email address.");

    [Fact(DisplayName = "InclusiveBetween() returns correct message")]
    public void InclusiveBetween_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).InclusiveBetween(5, 8))
            .ErrorList[nameof(Person.AnotherInt)]["range"].Should().Be("'Another Int' must be between 5 and 8.");

    [Fact(DisplayName = "ExclusiveBetween() returns correct message")]
    public void ExclusiveBetween_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.AnotherInt).ExclusiveBetween(5, 8))
            .ErrorList[nameof(Person.AnotherInt)]["exclusiveRange"].Should().Be("'Another Int' must be between 5 and 8 (exclusive).");

    [Fact(DisplayName = "Child validator returns correct message")]
    public void ChildValidator_ReturnsCorrectMessage() =>
        TestListingHelper.GetValidatorRules(v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()))
            .ErrorList[$"{nameof(Address)}.{nameof(Address.Line1)}"]["required"].Should().Be("'Line1' must not be empty.");
}
