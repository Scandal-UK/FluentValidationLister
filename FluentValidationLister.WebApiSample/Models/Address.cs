// <copyright file="Address.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Models;

/// <summary>Simple class to represent the address.</summary>
public class Address
{
    /// <summary>Gets or sets address line 1.</summary>
    public string Line1 { get; set; }

    /// <summary>Gets or sets optional address line 2.</summary>
    public string Line2 { get; set; }

    /// <summary>Gets or sets the town.</summary>
    public string Town { get; set; }

    /// <summary>Gets or sets the county.</summary>
    public string County { get; set; }

    /// <summary>Gets or sets the postcode.</summary>
    public string Postcode { get; set; }
}
