// <copyright file="Optional.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample.Models;

/// <summary> Model without validator. </summary>
public class Optional
{
    /// <summary> Gets or sets the Id. </summary>
    public int Id { get; set; }

    /// <summary> Gets or sets the Name. </summary>
    public string Name { get; set; }

    /// <summary> Gets or sets the Creation Date. </summary>
    public DateTimeOffset CreationDate { get; set; }

    /// <summary> Gets or sets a value indicating whether is enabled. </summary>
    public bool IsEnabled { get; set; }
}
