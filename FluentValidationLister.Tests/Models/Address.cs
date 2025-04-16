// <copyright file="Address.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Models;

public class Address : IAddress
{
    /// <inheritdoc/>
    public string Line1 { get; set; }

    /// <inheritdoc/>
    public string Line2 { get; set; }

    /// <inheritdoc/>
    public string Town { get; set; }

    /// <inheritdoc/>
    public string County { get; set; }

    /// <inheritdoc/>
    public string Postcode { get; set; }

    /// <inheritdoc/>
    public Country Country { get; set; }

    /// <summary> Gets or sets the ID. </summary>
    public int Id { get; set; }
}
