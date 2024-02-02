// <copyright file="TestListingTypes.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests;

using FluentAssertions;
using FluentValidationLister.Tests.Models;
using Xunit;

/// <summary> Tests for the list of target types. </summary>
public class TestListingTypes
{
    /// <summary> Check that "string" is returned for the appropriate type. </summary>
    [Fact(DisplayName = "Types include string")]
    public void Types_IncludeString() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Surname)]
        .Should().Be("string");

    /// <summary> Check that "number" is returned for the appropriate type. </summary>
    [Fact(DisplayName = "Types include number")]
    public void Types_IncludeNumber() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Id)]
        .Should().Be("number");

    /// <summary> Check that "boolean" is returned for the appropriate type. </summary>
    [Fact(DisplayName = "Types include boolean")]
    public void Types_IncludeBoolean() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.IsActive)]
        .Should().Be("boolean");

    /// <summary> Check that "date" is returned for the appropriate type. </summary>
    [Fact(DisplayName = "Types include date")]
    public void Types_IncludeDate() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.DateOfBirth)]
        .Should().Be("date");

    /// <summary> Check that "string" is returned for children of the appropriate type. </summary>
    [Fact(DisplayName = "Types include child types")]
    public void Types_IncludeChildTypes() =>
        TestListingHelper.GetValidatorRules().TypeList[$"{nameof(Address)}.{nameof(Address.Line1)}"]
        .Should().Be("string");

    /// <summary> Check that "number" is returned for children of the appropriate type. </summary>
    [Fact(DisplayName = "Types include collection types")]
    public void Types_IncludeCollectionTypes() =>
        TestListingHelper.GetValidatorRules().TypeList[$"{nameof(Person.Orders)}.{nameof(Order.Amount)}"]
        .Should().Be("number");

    /// <summary> Check that "number" is returned for nullable integer. </summary>
    [Fact(DisplayName = "Types; Integer? returns number")]
    public void Types_IntegerReturnsNumber() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.NullableInt)]
        .Should().Be("number");

    /// <summary> Check that "number" is returned for nullable decimal. </summary>
    [Fact(DisplayName = "Types; Decimal? returns number")]
    public void Types_DecimalReturnsNumber() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Discount)]
        .Should().Be("number");

    /// <summary> Check that "number" is returned for nullable double. </summary>
    [Fact(DisplayName = "Types; Double? returns number")]
    public void Types_DoubleReturnsNumber() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.Age)]
        .Should().Be("number");

    /// <summary> Check that "number" is returned for nullable float. </summary>
    [Fact(DisplayName = "Types; Float? returns number")]
    public void Types_FloatOrSingleReturnsNumber() =>
        TestListingHelper.GetValidatorRules().TypeList[nameof(Person.SomeSingleValue)]
        .Should().Be("number");
}
