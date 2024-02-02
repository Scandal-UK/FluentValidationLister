// <copyright file="ServiceCollectionExtensions.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Filter.DependencyInjection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

/// <summary> Class to encapsulate dependency injection methods. </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a global <see cref="IActionFilter"/> for returning serialised lists of validation rules and messages.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further customisation.</returns>
    public static IServiceCollection AddFluentValidationLister(this IServiceCollection services) =>
        services
            .Configure<MvcOptions>(o =>
            {
                o.Filters.Add(typeof(ValidationActionFilter));
            });
}
