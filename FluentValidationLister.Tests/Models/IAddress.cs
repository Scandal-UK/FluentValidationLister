// <copyright file="IAddress.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Models;

public interface IAddress
{
    /// <summary> Gets or sets line 1. </summary>
    string Line1 { get; set; }

    /// <summary> Gets or sets line 2. </summary>
    string Line2 { get; set; }

    /// <summary> Gets or sets town. </summary>
    string Town { get; set; }

    /// <summary> Gets or sets county/region. </summary>
    string County { get; set; }

    /// <summary> Gets or sets postcode. </summary>
    string Postcode { get; set; }

    /// <summary> Gets or sets country. </summary>
    Country Country { get; set; }
}
