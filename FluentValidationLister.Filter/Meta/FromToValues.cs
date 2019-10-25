namespace FluentValidationLister.Filter.Meta
{
    using System;

    /// <summary>
    /// Class to hold serialisable properties for From and To.
    /// </summary>
    public class FromToValues
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FromToValues"/> class.
        /// </summary>
        public FromToValues()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FromToValues"/> class.
        /// </summary>
        /// <param name="from">Value for the From property.</param>
        /// <param name="to">Value for the To property.</param>
        public FromToValues(IComparable from, IComparable to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>Gets or sets the From value.</summary>
        public IComparable From { get; set; }

        /// <summary>Gets or sets the To value.</summary>
        public IComparable To { get; set; }
    }
}
