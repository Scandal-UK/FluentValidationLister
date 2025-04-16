// <copyright file="Order.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Models;

public class Order : IOrder
{
    /// <summary> Gets or sets the product name. </summary>
    public string ProductName { get; set; }

    /// <inheritdoc/>
    public decimal Amount { get; set; }
}
