// <copyright file="FromToValues.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Filter.Meta;

using System;

/// <summary>
/// Class to hold serialisable properties for From and To.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="FromToValues"/> class with specified
/// values for the From and To properties.
/// </remarks>
/// <param name="from">Value for the From property.</param>
/// <param name="to">Value for the To property.</param>
public class FromToValues(IComparable from, IComparable to)
{
    /// <summary>Gets or sets the From value.</summary>
    public int From { get; set; } = from is int @int ? @int : throw new InvalidOperationException($"Cannot set range for type {from.GetType()}");

    /// <summary>Gets or sets the To value.</summary>
    public int To { get; set; } = to is int int1 ? int1 : throw new InvalidOperationException($"Cannot set range for type {to.GetType()}");
}
