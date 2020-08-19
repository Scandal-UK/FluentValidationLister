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

        /// <summary>Gets or sets a list of types indexed by property name.</summary>
        public SerialisableDictionary<string, string> TypeList { get; set; } = new SerialisableDictionary<string, string>();
    }
}
