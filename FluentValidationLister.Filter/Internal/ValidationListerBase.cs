// <copyright file="ValidationListerBase.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.Filter.Internal;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using FluentValidationLister.Filter.Meta;

/// <summary>
/// A base class to inspect an instance of <see cref="IValidator"/> and produce a list of
/// rules, types and failure messages based on the types specified in the derivatives.
/// </summary>
public abstract class ValidationListerBase(IValidator validator, Type modelType, IServiceProvider serviceProvider)
{
    private static readonly HashSet<Type> NumericTypes =
    [
        typeof(int),  typeof(double), typeof(decimal),
        typeof(long), typeof(short),  typeof(sbyte),
        typeof(byte), typeof(ulong),  typeof(ushort),
        typeof(uint), typeof(float),  typeof(BigInteger),
    ];

    private readonly IValidatorDescriptor validatorDescriptor = validator?.CreateDescriptor() ?? throw new ArgumentNullException(nameof(validator));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private ValidatorRules rules;

    /// <summary> Gets the <see cref="Type"/> of the model that is being validated. </summary>
    public Type ModelType { get; } = modelType ?? throw new ArgumentNullException(nameof(modelType));

    /// <summary>Instantiates an instance of the <see cref="ValidatorRules"/> class.</summary>
    /// <returns>Instance of <see cref="ValidatorRules"/>.</returns>
    public ValidatorRules GetValidatorRules()
    {
        this.rules = new ValidatorRules();
        foreach (var member in this.validatorDescriptor?.GetMembersWithValidators())
        {
            this.AddRulesForMember(this.validatorDescriptor, member.Key);
        }

        this.AddPropertyTypes(this.ModelType);

        return this.rules;
    }

    /// <summary> Override for adding a rule based on the validator type. </summary>
    /// <param name="rule">An instance of <see cref="IValidationRule"/>.</param>
    /// <param name="component">An instance of <see cref="IRuleComponent"/>.</param>
    /// <param name="propertyName">The property name.</param>
    internal abstract void AddRuleBasedOnValidatorType(IValidationRule rule, IRuleComponent component, string propertyName);

    /// <summary> Method to add a rule using all relevant properties. </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="errorMessageTemplate">The template string, if exists.</param>
    /// <param name="validatorType">The validator type.</param>
    /// <param name="validatorValue">The validator value.</param>
    /// <param name="additionalArguments">Additional argument(s) as required.</param>
    internal void AddRule(string propertyName, string displayName, string errorMessageTemplate, string validatorType, object validatorValue, params (string, object)?[] additionalArguments)
    {
        this.AddValidator(propertyName, validatorType, validatorValue);
        this.AddErrorMessage(propertyName, validatorType, displayName, errorMessageTemplate, additionalArguments);
    }

    private static bool IsDuplicatedPropertyName(string propertyName)
    {
        var parts = propertyName.Split('.');
        return parts.Length > 1 && parts[^1] == parts[^2];
    }

    private static string DeriveJsonTypeFromType(Type dataType)
    {
        if (NumericTypes.Contains(dataType))
        {
            return "number";
        }

        return dataType.Name switch
        {
            "Boolean" => "boolean",
            "DateTime" => "date",
            "DateTimeOffset" => "date",
            "String" => "string",
            _ => "object",
        };
    }

    private static string BuildErrorMessage(string displayName, string errorMessageTemplate, params (string, object)?[] additionalArguments)
    {
        // Discard second sentences which rely on users input (e.g. "you entered {TotalLength} characters")
        if (errorMessageTemplate.Contains("{TotalLength}") || errorMessageTemplate.Contains("{PropertyValue}"))
        {
            errorMessageTemplate = errorMessageTemplate[..(errorMessageTemplate.IndexOf('.') + 1)];
        }

        var formatter = new MessageFormatter();
        formatter.AppendPropertyName(displayName);

        foreach (var argument in additionalArguments)
        {
            formatter.AppendArgument(argument.Value.Item1, argument.Value.Item2);
        }

        return formatter.BuildMessage(errorMessageTemplate);
    }

    private void AddPropertyTypes(Type targetType, string targetPrefix = "")
    {
        foreach (var prop in targetType.GetProperties())
        {
            this.AddPropertyTypeOrDescendantPropertyTypes(
                Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType,
                $"{targetPrefix}{prop.Name}");
        }
    }

    private void AddPropertyTypeOrDescendantPropertyTypes(Type propertyType, string propertyName)
    {
        var jsonType = DeriveJsonTypeFromType(propertyType);
        if (propertyType.IsPrimitive || jsonType != "object" || IsDuplicatedPropertyName(propertyName))
        {
            this.AddType(propertyName, jsonType);
        }
        else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
        {
            this.AddPropertyTypes(propertyType.GetGenericArguments()[0], propertyName + '.');
        }
        else if (propertyType.IsClass && !propertyType.IsGenericType)
        {
            this.AddPropertyTypes(propertyType, propertyName + '.');
        }
    }

    private void AddRulesForMember(IValidatorDescriptor descriptor, string memberName, string propertyPrefix = "")
    {
        var validationRules = descriptor
            .GetRulesForMember(memberName);

        foreach (var rule in validationRules)
        {
            foreach (var component in rule.Components)
            {
                this.AddRuleOrDescendantRules(rule, component, $"{propertyPrefix}{memberName}");
            }
        }
    }

    private void AddRuleOrDescendantRules(IValidationRule rule, IRuleComponent component, string propertyName)
    {
        if (component.Validator is IChildValidatorAdaptor childValidatorAdaptor)
        {
            var childValidator = this.FetchChildValidatorFromIoc(childValidatorAdaptor);
            var childDescriptor = childValidator.CreateDescriptor();

            foreach (var member in childDescriptor.GetMembersWithValidators())
            {
                this.AddRulesForMember(childDescriptor, member.Key, propertyName + '.');
            }
        }
        else
        {
            this.AddRuleBasedOnValidatorType(rule, component, propertyName);
        }
    }

    private void AddValidator(string propertyName, string validatorType, object validatorValue)
    {
        if (!this.rules.ValidatorList.TryGetValue(propertyName, out _))
        {
            this.rules.ValidatorList.Add(propertyName, []);
        }

        this.rules.ValidatorList[propertyName].Add(validatorType, validatorValue);
    }

    private void AddType(string propertyName, string jsonDataType)
    {
        if (!this.rules.TypeList.TryAdd(propertyName, jsonDataType))
        {
            this.rules.TypeList[propertyName] = jsonDataType;
        }
    }

    private void AddErrorMessage(string propertyName, string validatorType, string displayName, string errorMessageTemplate, params (string, object)?[] additionalArguments)
    {
        if (!this.rules.ErrorList.TryGetValue(propertyName, out _))
        {
            this.rules.ErrorList.Add(propertyName, []);
        }

        this.rules.ErrorList[propertyName].Add(validatorType, BuildErrorMessage(displayName, errorMessageTemplate, additionalArguments));
    }

    private IValidator FetchChildValidatorFromIoc(IChildValidatorAdaptor childValidatorAdaptor) =>
        (IValidator)this.serviceProvider.GetService(typeof(IValidator<>)
            .MakeGenericType(childValidatorAdaptor.GetType().GenericTypeArguments[1]));
}
