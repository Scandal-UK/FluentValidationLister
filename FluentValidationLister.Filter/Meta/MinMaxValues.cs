namespace FluentValidationLister.Filter.Meta
{
    /// <summary>
    /// Class to hold serialisable properties for Min and Max.
    /// </summary>
    public class MinMaxValues
    {
        /// <summary>Initialises a new instance of the <see cref="MinMaxValues"/> class.</summary>
        public MinMaxValues()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MinMaxValues"/> class with specified
        /// values for the Min and Max properties.
        /// </summary>
        /// <param name="min">Value of the minimum property.</param>
        /// <param name="max">Value of the maximum property.</param>
        public MinMaxValues(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>Gets or sets the minimum value.</summary>
        public int Min { get; set; }

        /// <summary>Gets or sets the maximum value.</summary>
        public int Max { get; set; }
    }
}
