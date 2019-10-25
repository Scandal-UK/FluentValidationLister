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
        public AddressValidator()
            : this(v => v.RuleFor(x => x.Line1).NotEmpty())
        {
            var countryValidator = new CountryValidator(v => v.RuleFor(x => x.Name).NotEmpty());
            this.RuleFor(x => x.Country).SetValidator(countryValidator);
        }

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
        public OrderValidator()
            : this(v => v.RuleFor(x => x.Amount).NotNull())
        {
        }

        public OrderValidator(params Action<OrderValidator>[] actions)
        {
            foreach (var action in actions)
                action(this);
        }
    }
}
