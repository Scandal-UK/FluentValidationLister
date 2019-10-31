namespace FluentValidationLister.Filter
{
    using System;
    using System.Linq;
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using FluentValidationLister.Filter.Internal;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public sealed partial class ValidationActionFilter
        : IActionFilter
    {
        /// <summary>
        /// Execution of an action on any controller.
        /// </summary>
        /// <param name="context">Current executing context.</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
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

            var validator = context.HttpContext.RequestServices
                .GetService(typeof(IValidator<>)
                .MakeGenericType(GetModelType(model))) as IValidator;

            // Return validation rules if requested
            if (context.HttpContext.Request.Query.Keys.Any(val => val == "validation"))
            {
                if (validator == null)
                {
                    context.Result = new EmptyResult();
                }
                else
                {
                    context.Result = new OkObjectResult(new ValidationLister(validator).GetValidatorRules());
                }

                return;
            }
            else if (validator == null)
            {
                return;
            }

            // Validate the model
            var validationResult = validator.Validate(new ValidationContext(model));
            validationResult.AddToModelState(context.ModelState, null);

            if (!context.ModelState.IsValid)
            {
                // Force camel cased keys (if the model was attributed with [FromForm], the keys won't be camel-cased)
                var modelState = context.ModelState.ToDictionary(
                    p => p.Key.ToCamelCase(),
                    p => p.Value.Errors.Select(x => x.ErrorMessage).ToArray());

                // Check if single-field validation is being requested
                if (context.HttpContext.Request.Query.TryGetValue("validate", out var validateField))
                {
                    // Remove all the other fields from the result
                    foreach (var item in modelState.Where(p => p.Key.ToLowerInvariant() != validateField.ToString().ToLowerInvariant()).ToList())
                    {
                        modelState.Remove(item.Key);
                    }

                    // If the selected field is not invalid, return an empty OK result
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
