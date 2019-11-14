namespace FluentValidationLister.Filter.Meta
{
    using FluentValidationLister.Filter.Internal;

    /// <summary>
    /// A class to hold lists of rules and messages, with the intention of being serialised.
    /// </summary>
    public class ValidatorRules
    {
        /// <summary>Gets or sets a list of validator rules indexed by property name.</summary>
        public SerialisableDictionary<string, SerialisableDictionary<string, object>> ValidatorList { get; set; } = new SerialisableDictionary<string, SerialisableDictionary<string, object>>();

        /// <summary>Gets or sets a list of error messages indexed by property name.</summary>
        public SerialisableDictionary<string, SerialisableDictionary<string, string>> ErrorList { get; set; } = new SerialisableDictionary<string, SerialisableDictionary<string, string>>();

        public void ConvertPropertyNamesToCamelCase()
        {
            var newValidatorList = new SerialisableDictionary<string, SerialisableDictionary<string, object>>();
            foreach (var item in this.ValidatorList)
                newValidatorList.Add(item.Key.ToCamelCase(), this.ValidatorList[item.Key]);

            var newErrorList = new SerialisableDictionary<string, SerialisableDictionary<string, string>>();
            foreach (var item in this.ErrorList)
                newErrorList.Add(item.Key.ToCamelCase(), this.ErrorList[item.Key]);

            this.ValidatorList = newValidatorList;
            this.ErrorList = newErrorList;
        }
    }
}
