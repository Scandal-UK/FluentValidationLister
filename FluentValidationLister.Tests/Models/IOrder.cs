// <copyright file="IOrder.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests.Models;

/// <summary> Interface defining fields required for the order object. </summary>
public interface IOrder
{
    /// <summary> Gets the amount. </summary>
    decimal Amount { get; }
}
