namespace FluentValidationLister.Filter
{
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

    public sealed partial class ValidationActionFilter
        : IActionFilter
    {
        /// <summary>Execution of an action on any controller.</summary>
        /// <param name="context">Current executing context.</param>
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

            if (!(context.HttpContext.RequestServices
                .GetService(typeof(IValidator<>)
                .MakeGenericType(GetModelType(model))) is IValidator validator))
            {
                // No validator is applicable for this endpoint
                if (requestingValidatorList)
                {
                    context.Result = new EmptyResult();
                }

                return;
            }

            // Return list of validation rules
            if (requestingValidatorList)
            {
                // todo: consider passing type here so we can list all property types
                var rules = new ValidationLister(validator, GetModelType(model), context.HttpContext.RequestServices).GetValidatorRules();
                this.ConvertPropertyNamesToCamelCase(rules);

                context.Result = new OkObjectResult(rules);
                return;
            }

            // Validate the model
            this.ValidateModel(validator, model, context.ModelState);
            if (!context.ModelState.IsValid)
            {
                // Force camel-cased keys (if the model was attributed with [FromForm], the keys won't be camel-cased)
                var modelState = context.ModelState.ToDictionary(
                    p => p.Key.ToCamelCase(),
                    p => p.Value.Errors.Select(x => x.ErrorMessage).ToArray());

                // Check if single-field AJAX validation is being requested
                if (context.HttpContext.Request.Query.TryGetValue("validate", out var validateField))
                {
                    // Remove all the other fields from the result
                    foreach (var item in modelState.Where(p => p.Key.ToLowerInvariant() != validateField.ToString().ToLowerInvariant()).ToList())
                    {
                        modelState.Remove(item.Key);
                    }

                    // If the selected field is valid, return an empty OK result
                    if (modelState.Count == 0)
                    {
                        context.Result = new EmptyResult();
                        return;
                    }
                }
                else
                {
                    // Remove the valid items from the dictionary
                    foreach (var item in modelState.Where(p => p.Value.Length == 0).ToList())
                    {
                        modelState.Remove(item.Key);
                    }
                }

                // Return a problem details response
                context.HttpContext.Response.ContentType = "application/problem+json";
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState)
                {
                    Detail = "Model validation error",
                });
            }
        }

        private void ConvertPropertyNamesToCamelCase(ValidatorRules validatorRules)
        {
            var newValidatorList = new Dictionary<string, Dictionary<string, object>>();
            foreach (var item in validatorRules.ValidatorList)
                newValidatorList.Add(item.Key.ToCamelCase(), validatorRules.ValidatorList[item.Key]);

            var newErrorList = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in validatorRules.ErrorList)
                newErrorList.Add(item.Key.ToCamelCase(), validatorRules.ErrorList[item.Key]);

            var newTypeList = new Dictionary<string, string>();
            foreach (var item in validatorRules.TypeList)
                newTypeList.Add(item.Key.ToCamelCase(), validatorRules.TypeList[item.Key]);

            validatorRules.ValidatorList = newValidatorList;
            validatorRules.ErrorList = newErrorList;
            validatorRules.TypeList = newTypeList;
        }

        private void ValidateModel(IValidator validator, object model, ModelStateDictionary modelState)
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

        /// <summary>
        /// Event that fires after an action has finished execution.
        /// </summary>
        /// <param name="context">Context of executed action.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        private static object ActionArgumentAsModel(ActionExecutingContext actionContext) =>
            (from arg in actionContext.ActionArguments.Values
             let typ = arg?.GetType()
             where typ != null && !typ.IsPrimitive && !typ.IsValueType && typ != typeof(string)
             select arg).FirstOrDefault();

        private static Type GetModelType(object model) =>
            model.GetType().IsGenericType ? model.GetType().GenericTypeArguments.FirstOrDefault() : model.GetType();
    }
}
