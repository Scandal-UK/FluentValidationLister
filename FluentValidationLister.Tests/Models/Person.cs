// <copyright file="Person.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Models;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary> Dummy entity for testing purposes. </summary>
/// <remarks><see href="https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation.Tests/Person.cs">Original source</see>.</remarks>
public class Person
{
    /// <summary> Gets or sets the ID. </summary>
    public int Id { get; set; }

    /// <summary> Gets or sets the surname. </summary>
    public string Surname { get; set; }

    /// <summary> Gets or sets the forename. </summary>
    public string Forename { get; set; }

    /// <summary> Gets or sets a value indicating whether is active. </summary>
    public bool IsActive { get; set; } = true;

    /// <summary> Gets or sets the children records. </summary>
    public List<Person> Children { get; set; } = [];

    /// <summary> Gets or sets the date of birth. </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary> Gets or sets the nullable int. </summary>
    public int? NullableInt { get; set; }

    /// <summary> Gets or sets the address. </summary>
    public Address Address { get; set; }

    /// <summary> Gets or sets the order records. </summary>
    public List<Order> Orders { get; set; } = [];

    /// <summary> Gets or sets the email. </summary>
    public string Email { get; set; }

    /// <summary> Gets or sets the discount. </summary>
    public decimal? Discount { get; set; }

    /// <summary> Gets or sets the age. </summary>
    public double? Age { get; set; }

    /// <summary> Gets or sets another int figure. </summary>
    public int AnotherInt { get; set; }

    /// <summary> Gets or sets the credit card. </summary>
    public string CreditCard { get; set; }

    /// <summary> Gets or sets the other nullable int figure. </summary>
    public int? OtherNullableInt { get; set; }

    /// <summary> Gets or sets the regex. </summary>
    public string Regex { get; set; }

    /// <summary> Gets or sets another regex. </summary>
    public Regex AnotherRegex { get; set; }

    /// <summary> Gets or sets the minimum length. </summary>
    public int MinLength { get; set; }

    /// <summary> Gets or sets the maximum length. </summary>
    public int MaxLength { get; set; }

    /// <summary> Gets or sets the gender. </summary>
    public EnumGender Gender { get; set; }

    /// <summary> Gets or sets the gender string. </summary>
    public string GenderString { get; set; }

    /// <summary> Gets the forename in a readonly manner. </summary>
    public string ForenameReadOnly => this.Forename;

    /// <summary> Gets or sets some single value - defaults to 1. </summary>
    public float? SomeSingleValue { get; set; } = 1.0F;

    /// <summary> Calculate and return salary figure. </summary>
    /// <returns>Salary amount.</returns>
    public static int CalculateSalary()
    {
        return 20;
    }
}
