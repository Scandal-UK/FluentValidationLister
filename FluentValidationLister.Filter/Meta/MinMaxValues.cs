// <copyright file="MinMaxValues.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Filter.Meta;

/// <summary>
/// Class to hold serialisable properties for Min and Max.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="MinMaxValues"/> class with specified
/// values for the Min and Max properties.
/// </remarks>
/// <param name="min">Value of the minimum property.</param>
/// <param name="max">Value of the maximum property.</param>
public class MinMaxValues(int min, int max)
{
    /// <summary>Gets or sets the minimum value.</summary>
    public int Min { get; set; } = min;

    /// <summary>Gets or sets the maximum value.</summary>
    public int Max { get; set; } = max;
}
