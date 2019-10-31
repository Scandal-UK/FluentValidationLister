# FluentValidationLister

This project adds an Action Filter which will describe the rules and messages defined in a particular FluentValidation validator.

This has been designed using ASP.NET Core 2.2.7 and FluentValidation 8.5.0. It is also fully compatible with ASP.NET Core 3.

## Table of Contents

- [Purpose](#purpose)
- [Installation](#installation)
- [How to use](#how-to-use)
  - [Rules](#how-to-use---rules)
  - [AJAX validation](#how-to-use---ajax-validation)

## Purpose

If you want to use clientside validation with the FluentValidation library (you really should), then the recommended way is to use the FluentValidation.AspNetCore extension with ASP.NET Core MVC. It integrates perfectly and there's no need for this filter at all.

However, _sometimes_ you may not be using MVC - you might be using an SPA application, such as React or Angular, or maybe a mobile application front-end. In this case you should probably want your clientside validators to match your server-side FluentValidtion validators, without having to duplicate the effort.

You still need to implement this - it just provides a nice way to access the validator information for any endpoint, exposing and formatting the metadata provided by the FluentValidation library.

This solution includes an ASP.NET Core 3 Web API application which provides a very basic (jQuery-based) AJAX example of revealing the validator information for any given endpoint, although it is probably easier to test it using a tool such as Postman.


## Installation

1. Install the [NuGet package](https://www.nuget.org/packages/FluentValidationLister.Filter/)

2. In the _ConfigureServices_ method of _Startup.cs_, include a call to `AddFluentValidationFilter` instead of `AddFluentValidation`.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddFluentValidationFilter();
}
```

3. In order for ASP.NET to discover your validators, they must be registered with the services collection. You must do this by calling the `AddTransient` method for each of your validators. Adding all validators in a specified assembly is not supported.


```csharp
    services.AddFluentValidationFilter();

    services.AddTransient<IValidator<Person>, PersonValidator>();
    // (repeat for every validator)
```

## How to use

For any given endpoint, add the query string _validation=1_ in order to view the validator details for that endpoint.

Example output for JSON;

```json
{
  "rules": {
    "foreName": {
      "required": true,
      "length": {
        "min": 2,
        "max": 10
      }
    },
    "address.line1": {
      "required": true
    }
  },
  "messages": {
    "foreName": {
      "required": "'Forename' must not be empty.",
      "length": "'Forename' must be between 2 and 10 characters."
    },
    "address.line1": {
      "required": "'Line1' must not be empty."
    }
  }
}
```

### How to use - Rules

The following validator rules are presented to the client:

- NotNull/NotEmpty (required)
- Matches (regex)
- InclusiveBetween (range)
- ExclusiveBetween (exclusiveRange)
- Email
- EqualTo (cross-property equality comparison)
- MaxLength
- MinLength
- Length (including exactLength)

The following validators are presented as "remote" to the client (they can be validated using AJAX, see below):

- CreditCard
  - https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/CreditCardAttribute.cs
  - https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/CreditCardValidator.cs
- Phone
  - https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/PhoneAttribute.cs
- ScalePrecision (e.g. .Numeric(scale, precision); )
- NotEqual
- Enum

### How to use - AJAX validation

For unsupported validators or validators with custom server-side logic, the rule will be presented as "remote" - the field can be validated using an AJAX call to the back-end.

You can validate any particular field by adding the query string _validate=fieldName_ and if the field is not valid, a problem details will be returned including only the specified field.
