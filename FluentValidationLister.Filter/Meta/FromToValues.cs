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
        /// Initialises a new instance of the <see cref="FromToValues"/> class with specified
        /// values for the From and To properties.
        /// </summary>
        /// <param name="from">Value for the From property.</param>
        /// <param name="to">Value for the To property.</param>
        public FromToValues(IComparable from, IComparable to)
        {
            this.From = from is int ? (int)from : throw new InvalidOperationException($"Cannot set range for type {from.GetType()}");
            this.To = to is int ? (int)to : throw new InvalidOperationException($"Cannot set range for type {to.GetType()}");
        }

        /// <summary>Gets or sets the From value.</summary>
        public int From { get; set; }

        /// <summary>Gets or sets the To value.</summary>
        public int To { get; set; }
    }
}
