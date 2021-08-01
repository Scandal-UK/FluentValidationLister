namespace FluentValidationLister.Filter.Meta
{
    using System.Collections.Generic;

    /// <summary>
    /// A class to hold lists of rules and messages, with the intention of being serialised.
    /// </summary>
    public class ValidatorRules
    {
        /// <summary>Gets or sets a list of validator rules indexed by property name.</summary>
        public Dictionary<string, Dictionary<string, object>> ValidatorList { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>Gets or sets a list of error messages indexed by property name.</summary>
        public Dictionary<string, Dictionary<string, string>> ErrorList { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>Gets or sets a list of types indexed by property name.</summary>
        public Dictionary<string, string> TypeList { get; set; } = new Dictionary<string, string>();
    }
}
