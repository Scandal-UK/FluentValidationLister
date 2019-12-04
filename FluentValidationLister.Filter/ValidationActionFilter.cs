namespace FluentValidationLister.Filter
{
    using System;
    using System.Buffers;
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
        /// <summary>
        /// Execution of an action on any controller.
        /// </summary>
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

            var validator = context.HttpContext.RequestServices
                .GetService(typeof(IValidator<>)
                .MakeGenericType(GetModelType(model))) as IValidator;

            // todo: Determine if user has chosen Pascal case! Tricky as its different between ASPNETCore 2 & 3
            bool useJson = true;
            if (context.HttpContext.Request.ContentType != null)
            {
                useJson = context.HttpContext.Request.ContentType.ToLowerInvariant().Contains("json");
            }

            // Return validation rules if requested
            if (context.HttpContext.Request.Query.Keys.Any(val => val == "validation"))
            {
                if (validator == null)
                {
                    context.Result = new EmptyResult();
                }
                else
                {
                    var rules = new ValidationLister(validator).GetValidatorRules();
                    if (useJson)
                    {
                        this.ConvertPropertyNamesToCamelCase(rules);
                    }

                    context.Result = new OkObjectResult(rules);
                }

                return;
            }
            else if (validator == null)
            {
                return;
            }

            // Validate the model
            this.ValidateModel(validator, model, context.ModelState);
            if (!context.ModelState.IsValid)
            {
                // Force camel-cased keys for JSON responses (if the model was attributed with [FromForm], the keys won't be camel-cased)
                var modelState = context.ModelState.ToDictionary(
                    p => useJson ? p.Key.ToCamelCase() : p.Key,
                    p => p.Value.Errors.Select(x => x.ErrorMessage).ToArray());

                // Check if single-field AJAX validation is being requested
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
                context.HttpContext.Response.ContentType = string.Format("application/problem+{0}", useJson ? "json" : "xml");
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState)
                {
                    Detail = "Model validation error",
                });
            }
        }

        private void ConvertPropertyNamesToCamelCase(ValidatorRules validatorRules)
        {
            // Workaround as System.Text.Json in ASP.NET 3 cannot serialise property keys.
            // Although unnecessary for ASP.NET Core 2.2, it's too expensive to check!
            var newValidatorList = new SerialisableDictionary<string, SerialisableDictionary<string, object>>();
            foreach (var item in validatorRules.ValidatorList)
                newValidatorList.Add(item.Key.ToCamelCase(), validatorRules.ValidatorList[item.Key]);

            var newErrorList = new SerialisableDictionary<string, SerialisableDictionary<string, string>>();
            foreach (var item in validatorRules.ErrorList)
                newErrorList.Add(item.Key.ToCamelCase(), validatorRules.ErrorList[item.Key]);

            validatorRules.ValidatorList = newValidatorList;
            validatorRules.ErrorList = newErrorList;
        }

        private void ValidateModel(IValidator validator, object model, ModelStateDictionary modelState)
        {
            var result = validator.Validate(new ValidationContext(model));
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
