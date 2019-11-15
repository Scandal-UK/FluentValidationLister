namespace FluentValidationLister.Filter.Internal
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Class to provide additional functionality for strings.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Returns the string with the first character switched to lower case.
        /// </summary>
        /// <param name="input">Current string for which to make the first letter lower case.</param>
        /// <returns>Formatted string.</returns>
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
            {
                return input;
            }

            if (input.Contains("."))
            {
                var parts = input.Split('.');
                var camelCasedParts = new List<string>();
                foreach (var part in parts)
                {
                    camelCasedParts.Add(StringToCamelCase(part));
                }

                return string.Join(".", camelCasedParts);
            }

            return StringToCamelCase(input);
        }

        private static string StringToCamelCase(string input)
        {
            var chars = input.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    break;
                }

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }
    }
}
