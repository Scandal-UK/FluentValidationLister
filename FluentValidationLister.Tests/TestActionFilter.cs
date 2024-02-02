// <copyright file="TestActionFilter.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Tests;

using System.Collections.Generic;
using FluentAssertions;
using FluentValidationLister.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

/// <summary>
/// Unit tests for the action filter.
/// </summary>
public class TestActionFilter
{
    /// <summary>
    /// This is incomplete - I have been experimenting with a prototype unit test to test the filter.
    /// </summary>
    [Fact]
    public void ActionFilter_ReturnsJsonResult()
    {
        // Arrange
        var modelState = new ModelStateDictionary();

        var httpContextMock = new DefaultHttpContext();
        httpContextMock.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "validate", new StringValues("1") } });

        var validationFilter = new ValidationActionFilter();
        ActionExecutingContext actionExecutingContext = GetPlainActionExecutingContext(modelState, httpContextMock);

        // Act
        validationFilter.OnActionExecuting(actionExecutingContext);

        // Assert
        actionExecutingContext.Result.Should().BeOfType<EmptyResult>();
    }

    private static ActionExecutingContext GetPlainActionExecutingContext(ModelStateDictionary modelState, DefaultHttpContext httpContextMock)
    {
        var actionContext = new ActionContext(
            httpContextMock,
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            modelState);

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Mock.Of<Controller>());

        return actionExecutingContext;
    }
}
