namespace FluentValidationLister.Filter.Internal
{
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
                var childValidator = ExtractChildValidator(rule, childValidatorAdaptor);
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

        private static IValidator ExtractChildValidator(IValidationRule rule, IChildValidatorAdaptor validator)
        {
            Type t = validator.GetType();
            var methodName = nameof(ChildValidatorAdaptor<object, object>.GetValidator);
            var methodInfo = t.GetMethod(methodName);

            //var context = new PropertyValidatorContext(null, rule, rule.PropertyName, rule.PropertyFunc);
            return (IValidator)methodInfo.Invoke(validator, new object[] { null, rule.Member });
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

        internal abstract void AddRuleBasedOnValidatorType(IValidationRule rule, IRuleComponent component, string propertyName);

        internal void AddRule(string propertyName, string displayName, string errorMessageTemplate, string validatorType, object validatorValue, params (string, object)?[] additionalArguments)
        {
            this.AddValidator(propertyName, validatorType, validatorValue);
            this.AddErrorMessage(propertyName, validatorType, displayName, errorMessageTemplate, additionalArguments);
        }

        private void AddValidator(string propertyName, string validatorType, object validatorValue)
        {
            if (!this.rules.ValidatorList.ContainsKey(propertyName))
            {
                this.rules.ValidatorList.Add(propertyName, new Dictionary<string, object>());
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
                this.rules.ErrorList.Add(propertyName, new Dictionary<string, string>());
            }

            this.rules.ErrorList[propertyName].Add(validatorType, this.BuildErrorMessage(displayName, errorMessageTemplate, additionalArguments));
        }

        private string BuildErrorMessage(string displayName, string errorMessageTemplate, params (string, object)?[] additionalArguments)
        {
            // Discard second sentences which rely on users input (e.g. "you entered {TotalLength} characters")
            if (errorMessageTemplate.Contains("{TotalLength}") || errorMessageTemplate.Contains("{PropertyValue}"))
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
