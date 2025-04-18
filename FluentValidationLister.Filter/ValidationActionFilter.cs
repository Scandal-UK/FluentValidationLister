﻿// <copyright file="ValidationActionFilter.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Filter;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidationLister.Filter.Internal;
using FluentValidationLister.Filter.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary> Filter required to respond to requests regarding validation meta data. </summary>
public sealed partial class ValidationActionFilter : IActionFilter
{
    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null || context.HttpContext.Request.Method == "GET")
        {
            return;
        }

        var model = ActionArgumentAsModel(context);
        if (model == null)
        {
            if (context.HttpContext.Request.Query.Keys.Any(val => val == "validate"))
            {
                context.Result = new EmptyResult();
            }

            return;
        }

        bool requestingValidatorList = context.HttpContext.Request.Query.Keys.Any(val => val == "validation");

        if (context.HttpContext.RequestServices
            .GetService(typeof(IValidator<>)
            .MakeGenericType(GetModelType(model))) is not IValidator validator)
        {
            if (requestingValidatorList)
            {
                validator = null;
            }
            else
            {
                return;
            }
        }

        if (requestingValidatorList)
        {
            ReturnValidationLister(context, model, validator);
            return;
        }

        ValidateModel(validator, model, context.ModelState);

        if (!context.ModelState.IsValid)
        {
            var modelState = GetModelState(context);
            ReturnProblemDetailResponse(context, modelState);
        }
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        return;
    }

    private static void ReturnProblemDetailResponse(ActionExecutingContext context, Dictionary<string, string[]> modelState)
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState)
        {
            Detail = "Model validation error",
        });
    }

    private static Dictionary<string, string[]> GetModelState(ActionExecutingContext context)
    {
        // Force camel-cased keys (e.g. if the model attribute was decorated with [FromForm], the keys won't be camel-cased)
        var modelState = context.ModelState.ToDictionary(
            p => p.Key.ToCamelCase(),
            p => p.Value.Errors.Select(x => x.ErrorMessage).ToArray());

        if (context.HttpContext.Request.Query.TryGetValue("validate", out var validateField))
        {
            GetSingleFieldValidator(modelState, validateField);
        }
        else
        {
            RemoveValidItemsFromTheDictionary(modelState);
        }

        return modelState;
    }

    private static void RemoveValidItemsFromTheDictionary(Dictionary<string, string[]> modelState)
    {
        foreach (var item in modelState.Where(p => p.Value.Length == 0).ToList())
        {
            modelState.Remove(item.Key);
        }
    }

    private static void GetSingleFieldValidator(Dictionary<string, string[]> modelState, Microsoft.Extensions.Primitives.StringValues validateField)
    {
        foreach (var item in modelState.Where(p => !p.Key.Equals(validateField.ToString(), StringComparison.OrdinalIgnoreCase)).ToList())
        {
            modelState.Remove(item.Key);
        }
    }

    private static void ReturnValidationLister(ActionExecutingContext context, object model, IValidator validator)
    {
        var rules = new ValidationLister(
            validator,
            GetModelType(model),
            context.HttpContext.RequestServices).GetValidatorRules();

        ConvertPropertyNamesToCamelCase(rules);

        context.Result = new OkObjectResult(rules);
        return;
    }

    private static void ConvertPropertyNamesToCamelCase(ValidatorRules validatorRules)
    {
        var newValidatorList = new Dictionary<string, Dictionary<string, object>>();
        foreach (var item in validatorRules.ValidatorList)
        {
            newValidatorList.Add(item.Key.ToCamelCase(), validatorRules.ValidatorList[item.Key]);
        }

        var newErrorList = new Dictionary<string, Dictionary<string, string>>();
        foreach (var item in validatorRules.ErrorList)
        {
            newErrorList.Add(item.Key.ToCamelCase(), validatorRules.ErrorList[item.Key]);
        }

        var newTypeList = new Dictionary<string, string>();
        foreach (var item in validatorRules.TypeList)
        {
            newTypeList.Add(item.Key.ToCamelCase(), validatorRules.TypeList[item.Key]);
        }

        validatorRules.ValidatorList = newValidatorList;
        validatorRules.ErrorList = newErrorList;
        validatorRules.TypeList = newTypeList;
    }

    private static void ValidateModel(IValidator validator, object model, ModelStateDictionary modelState)
    {
        var result = validator.Validate(new ValidationContext<object>(model));
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                if (modelState.ContainsKey(error.PropertyName))
                {
                    modelState[error.PropertyName].Errors.Add(error.ErrorMessage);
                }
                else
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }
    }

    private static object ActionArgumentAsModel(ActionExecutingContext actionContext) =>
        (from arg in actionContext.ActionArguments.Values
         let typ = arg?.GetType()
         where typ != null && !typ.IsPrimitive && !typ.IsValueType && typ != typeof(string)
         select arg).FirstOrDefault();

    private static Type GetModelType(object model) =>
        model.GetType().IsGenericType ? model.GetType().GenericTypeArguments.FirstOrDefault() : model.GetType();
}
