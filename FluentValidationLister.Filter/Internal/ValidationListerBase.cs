namespace FluentValidationLister.Filter.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using FluentValidationLister.Filter.Meta;

    /// <summary>
    /// A base class to inspect an instance of <see cref="IValidator"/> and produce a list of
    /// rules and failure messages based on the types specified in the derivatives.
    /// </summary>
    public abstract class ValidationListerBase
    {
        private readonly IValidatorDescriptor validatorDescriptor;
        private ValidatorRules rules;

        /// <summary>
        /// Initialises a new instance of the <see cref="ValidationListerBase"/> class.
        /// </summary>
        public ValidationListerBase()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ValidationListerBase"/> class with a specified <see cref="IValidator"/>.
        /// </summary>
        /// <param name="validator">An instance of a FluentValidation <see cref="IValidator"/>.</param>
        protected ValidationListerBase(IValidator validator) =>
            this.validatorDescriptor = validator?.CreateDescriptor() ?? throw new ArgumentNullException(nameof(validator));

        /// <summary>
        /// Inspects the <see cref="IValidator"/> to produce a populated instance
        /// of the <see cref="ValidatorRules"/> class.
        /// </summary>
        /// <returns>Instance of <see cref="ValidatorRules"/>.</returns>
        public ValidatorRules GetValidatorRules()
        {
            this.rules = new ValidatorRules();
            foreach (var member in this.validatorDescriptor?.GetMembersWithValidators())
            {
                this.AddRulesForMember(this.validatorDescriptor, member.Key);
            }

            return this.rules;
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
            if (validator is ChildValidatorAdaptor childValidatorAdaptor)
            {
                var childValidator = childValidatorAdaptor.GetValidator(new PropertyValidatorContext(null, rule, rule.PropertyName));
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
                this.rules.ValidatorList.Add(propertyName, new Dictionary<string, object>());
            }

            this.rules.ValidatorList[propertyName].Add(validatorType, validatorValue);
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
    }
}
