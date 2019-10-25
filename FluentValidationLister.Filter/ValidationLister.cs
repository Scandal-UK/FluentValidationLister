namespace FluentValidationLister.Filter
{
    using System.Globalization;
    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Validators;
    using FluentValidationLister.Filter.Internal;
    using FluentValidationLister.Filter.Meta;

    /// <summary>
    /// A class to inspect an instance of <see cref="IValidator"/> and produce a list of rules and failure messages.
    /// </summary>
    public class ValidationLister : ValidationListerBase
    {
        /// <see href="https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/EmailValidator.cs"/>
        /// <remarks>ASP.NET Core and FluentValidation do not use a regex but we need to expose some client functionality that does the same thing.</remarks>
        private const string AspNetCoreCompatibleEmailAddressRegex = @"^[^@]+@[^@]+$";

        /// <see href="https://github.com/mono/aspnetwebstack/blob/master/src/Microsoft.Web.Mvc/EmailAddressAttribute.cs"/>
        /// <remarks>Got from framework source as FluentValidation uses an old broken version of the regex; <see href="https://github.com/JeremySkinner/FluentValidation/issues/1247"/>.</remarks>
        private const string Net4xEmailAddressRegex = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

        /// <summary>
        /// Initialises a new instance of the <see cref="ValidationLister"/> class.
        /// </summary>
        /// <param name="validator">An instance of <see cref="IValidator"/>.</param>
        public ValidationLister(IValidator validator)
            : base(validator)
        {
        }

        /// <summary>
        /// Inspect the validator type and add the rules/messages with the correct properties.
        /// </summary>
        /// <param name="rule">FluentValidation Property Rule.</param>
        /// <param name="validator">Validator to inspect.</param>
        /// <param name="propertyName">Name of the property to check.</param>
        internal override void AddRuleBasedOnValidatorType(PropertyRule rule, IPropertyValidator validator, string propertyName)
        {
            var displayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rule.GetDisplayName());
            var errorMessageTemplate = validator.Options.ErrorMessageSource.GetString(null);

            // This is the open extension point for OCP (Open Closed Principle)
            switch (validator)
            {
                case NotNullValidator _:
                case NotEmptyValidator _:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "required", true);
                    break;
                case AspNetCoreCompatibleEmailValidator _:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "regex", AspNetCoreCompatibleEmailAddressRegex);
                    break;
                case EmailValidator _:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "regex", Net4xEmailAddressRegex);
                    break;
                case RegularExpressionValidator expressionValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "regex", expressionValidator.Expression);
                    break;
                case EqualValidator equalValidator:
                    var comparisonValue = equalValidator.MemberToCompare?.Name ?? equalValidator.ValueToCompare;
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "compare", comparisonValue, ("ComparisonValue", comparisonValue));
                    break;
                case LessThanValidator lessThanValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "lessThan", lessThanValidator.ValueToCompare, ("ComparisonValue", lessThanValidator.ValueToCompare));
                    break;
                case LessThanOrEqualValidator lessThanOrEqualValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "lessThanOrEqualTo", lessThanOrEqualValidator.ValueToCompare, ("ComparisonValue", lessThanOrEqualValidator.ValueToCompare));
                    break;
                case GreaterThanValidator greaterThanValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "greaterThan", greaterThanValidator.ValueToCompare, ("ComparisonValue", greaterThanValidator.ValueToCompare));
                    break;
                case GreaterThanOrEqualValidator greaterThanOrEqualValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "greaterThanOrEqualTo", greaterThanOrEqualValidator.ValueToCompare, ("ComparisonValue", greaterThanOrEqualValidator.ValueToCompare));
                    break;
                case InclusiveBetweenValidator inclusiveBetweenValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "range", new FromToValues(inclusiveBetweenValidator.From, inclusiveBetweenValidator.To), ("From", inclusiveBetweenValidator.From), ("To", inclusiveBetweenValidator.To));
                    break;
                case ExclusiveBetweenValidator exclusiveBetweenValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "exclusiveRange", new FromToValues(exclusiveBetweenValidator.From, exclusiveBetweenValidator.To), ("From", exclusiveBetweenValidator.From), ("To", exclusiveBetweenValidator.To));
                    break;
                case ExactLengthValidator exactLengthValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "exactLength", exactLengthValidator.Max, ("MaxLength", exactLengthValidator.Max));
                    break;
                case MinimumLengthValidator minimumLengthValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "minLength", minimumLengthValidator.Min, ("MinLength", minimumLengthValidator.Min));
                    break;
                case MaximumLengthValidator maximumLengthValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "maxLength", maximumLengthValidator.Max, ("MaxLength", maximumLengthValidator.Max));
                    break;
                case LengthValidator lengthValidator:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "length", new MinMaxValues(lengthValidator.Min, lengthValidator.Max), ("MinLength", lengthValidator.Min), ("MaxLength", lengthValidator.Max));
                    break;
                default:
                    this.AddRule(propertyName, displayName, errorMessageTemplate, "remote", rule.PropertyName);
                    break;
            }
        }
    }
}
