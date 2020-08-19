namespace FluentValidationLister.WebApiSample.Models
{
    /// <summary>Entity to represent a person.</summary>
    public class Person
    {
        /// <summary>Gets or sets the primary key.</summary>
        public int? Id { get; set; }

        /// <summary>Gets or sets the surname.</summary>
        public string Surname { get; set; }

        /// <summary>Gets or sets the forename.</summary>
        public string Forename { get; set; }

        /// <summary>Gets or sets the age.</summary>
        public int? Age { get; set; }

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; }

        /// <summary>Gets or sets a value which indicates if the <see cref="Person"/> has agreed to sell their soul.</summary>
        public bool? SaleOfSoulAgreed { get; set; }

        /// <summary>Gets or sets the postal address.</summary>
        public Address Address { get; set; }
    }
}
