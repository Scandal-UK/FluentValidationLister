namespace FluentValidationLister.Tests.Samples
{
    using System;
    using FluentValidation;

    /// <summary>
    /// Validator for <see cref="Person"/> that supports rule setup at runtime.
    /// </summary>
    /// <remarks><see href="https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation.Tests/TestValidator.cs">Original source.</see></remarks>
    public class TestValidator : InlineValidator<Person>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TestValidator"/> class.
        /// </summary>
        /// <param name="actions"></param>
        public TestValidator(params Action<TestValidator>[] actions)
        {
            foreach (var action in actions)
                action(this);
        }
    }

    /// <summary>
    /// Validator for <see cref="Address"/> that supports rule setup at runtime.
    /// </summary>
    public class AddressValidator : InlineValidator<Address>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AddressValidator"/> class.
        /// </summary>
        public AddressValidator()
            : this(v => v.RuleFor(x => x.Line1).NotEmpty())
        {
            this.RuleFor(x => x.Country).SetInheritanceValidator(v => {
                v.Add<Country>(new CountryValidator());
            });
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AddressValidator"/> class.
        /// </summary>
        /// <param name="actions"></param>
        public AddressValidator(params Action<AddressValidator>[] actions)
        {
            foreach (var action in actions)
                action(this);
        }
    }

    /// <summary>
    /// Validator for <see cref="Country"/> that supports rule setup at runtime.
    /// </summary>
    public class CountryValidator : InlineValidator<Country>
    {
        /// <summary> Initialises a new instance of the <see cref="CountryValidator"/> class. </summary>
        public CountryValidator() => this.RuleFor(x => x.Name).NotEmpty();

        /// <summary> Initialises a new instance of the <see cref="CountryValidator"/> class. </summary>
        /// <param name="actions"></param>
        public CountryValidator(params Action<CountryValidator>[] actions)
        {
            foreach (var action in actions)
                action(this);
        }
    }

    /// <summary>
    /// Validator for <see cref="Order"/> that supports rule setup at runtime.
    /// </summary>
    public class OrderValidator : InlineValidator<Order>
    {
        /// <summary> Initialises a new instance of the <see cref="OrderValidator"/> class. </summary>
        public OrderValidator()
            : this(v => v.RuleFor(x => x.Amount).NotNull())
        {
        }

        /// <summary> Initialises a new instance of the <see cref="OrderValidator"/> class. </summary>
        /// <param name="actions"></param>
        public OrderValidator(params Action<OrderValidator>[] actions)
        {
            foreach (var action in actions)
                action(this);
        }
    }
}
