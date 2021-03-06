﻿namespace FluentValidationLister.Filter.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using FluentValidationLister.Filter.Meta;

    /// <summary>
    /// A base class to inspect an instance of <see cref="IValidator"/> and produce a list of
    /// rules, types and failure messages based on the types specified in the derivatives.
    /// </summary>
    public abstract class ValidationListerBase
    {
        private readonly IValidatorDescriptor validatorDescriptor;
        private ValidatorRules rules;

        /// <summary>
        /// Initialises a new instance of the <see cref="ValidationListerBase"/> class with a specified <see cref="IValidator"/>.
        /// </summary>
        /// <param name="validator">An instance of a FluentValidation <see cref="IValidator"/>.</param>
        /// <param name="modelType">The <see cref="Type"/> of the model being validated.</param>
        protected ValidationListerBase(IValidator validator, Type modelType)
        {
            this.validatorDescriptor = validator?.CreateDescriptor() ?? throw new ArgumentNullException(nameof(validator));
            this.ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
        }

        /// <summary>Gets the <see cref="Type"/> of the model that is being validated.</summary>
        public Type ModelType { get; }

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

        private static bool IsDuplicatedPropertyName(string propertyName)
        {
            var parts = propertyName.Split('.');
            return parts.Length > 1 && parts[parts.Length - 1] == parts[parts.Length - 2];
        }

        private void AddRulesForMember(IValidatorDescriptor descriptor, string memberName, string propertyPrefix = "")
        {
            var propertyRules = descriptor
                .GetRulesForMember(memberName)
                .OfType<PropertyRule>();

            foreach (var rule in propertyRules)
            {
                foreach (var validator in rule.Validators)
                {
                    this.AddRuleOrDescendantRules(rule, validator, $"{propertyPrefix}{memberName}");
                }
            }
        }

        private void AddRuleOrDescendantRules(PropertyRule rule, IPropertyValidator validator, string propertyName)
        {
            if (validator is IChildValidatorAdaptor)
            {
                var childValidator = ExtractChildValidator(rule, validator);
                var childDescriptor = childValidator.CreateDescriptor();

                foreach (var member in childDescriptor.GetMembersWithValidators())
                {
                    this.AddRulesForMember(childDescriptor, member.Key, propertyName + '.');
                }
            }
            else
            {
                this.AddRuleBasedOnValidatorType(rule, validator, propertyName);
            }
        }

        private static IValidator ExtractChildValidator(PropertyRule rule, IPropertyValidator validator)
        {
            Type t = validator.GetType();
            var methodName = nameof(ChildValidatorAdaptor<object, object>.GetValidator);
            var methodInfo = t.GetMethod(methodName);

            var context = new PropertyValidatorContext(null, rule, rule.PropertyName);
            return (IValidator)methodInfo.Invoke(validator, new object[] { context });
        }

        private static string DeriveJsonTypeFromType(Type dataType)
        {
            if (NumericTypes.Contains(dataType))
            {
                return "number";
            }

            switch (dataType.Name)
            {
                case "Boolean": return "boolean";
                case "DateTime": return "date";
                case "String": return "string";
                default: return "object";
            };
        }

        internal abstract void AddRuleBasedOnValidatorType(PropertyRule rule, IPropertyValidator validator, string propertyName);

        internal void AddRule(string propertyName, string displayName, string errorMessageTemplate, string validatorType, object validatorValue, params (string, object)?[] additionalArguments)
        {
            this.AddValidator(propertyName, validatorType, validatorValue);
            this.AddErrorMessage(propertyName, validatorType, displayName, errorMessageTemplate, additionalArguments);
        }

        private void AddValidator(string propertyName, string validatorType, object validatorValue)
        {
            if (!this.rules.ValidatorList.ContainsKey(propertyName))
            {
                this.rules.ValidatorList.Add(propertyName, new SerialisableDictionary<string, object>());
            }

            this.rules.ValidatorList[propertyName].Add(validatorType, validatorValue);
        }

        private void AddType(string propertyName, string jsonDataType)
        {
            if (!this.rules.TypeList.ContainsKey(propertyName))
            {
                this.rules.TypeList.Add(propertyName, jsonDataType);
            }
            else
            {
                this.rules.TypeList[propertyName] = jsonDataType;
            }
        }

        private void AddErrorMessage(string propertyName, string validatorType, string displayName, string errorMessageTemplate, params (string, object)?[] additionalArguments)
        {
            if (!this.rules.ErrorList.ContainsKey(propertyName))
            {
                this.rules.ErrorList.Add(propertyName, new SerialisableDictionary<string, string>());
            }

            this.rules.ErrorList[propertyName].Add(validatorType, this.BuildErrorMessage(displayName, errorMessageTemplate, additionalArguments));
        }

        private string BuildErrorMessage(string displayName, string errorMessageTemplate, params (string, object)?[] additionalArguments)
        {
            // Discard second sentences which rely on users input (e.g. "you entered {TotalLength} characters")
            if (errorMessageTemplate.Contains("{TotalLength}") || errorMessageTemplate.Contains("{Value}"))
            {
                errorMessageTemplate = errorMessageTemplate.Substring(0, errorMessageTemplate.IndexOf('.') + 1);
            }

            var formatter = new MessageFormatter();
            formatter.AppendPropertyName(displayName);

            foreach (var argument in additionalArguments)
            {
                formatter.AppendArgument(argument.Value.Item1, argument.Value.Item2);
            }

            return formatter.BuildMessage(errorMessageTemplate);
        }

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double), typeof(decimal),
            typeof(long), typeof(short),  typeof(sbyte),
            typeof(byte), typeof(ulong),  typeof(ushort),
            typeof(uint), typeof(float),  typeof(BigInteger)
        };
    }
}
