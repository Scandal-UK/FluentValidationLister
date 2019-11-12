# FluentValidationLister

This project adds an ActionFilter which will describe the rules and messages defined in a particular FluentValidation validator. It has been designed using FluentValidation v8.5.1.

It is fully compatible with ASP.NET Core 2.x and 3.x.

## Table of Contents

- [Purpose](#purpose)
- [Installation](#installation)
- [How to use](#how-to-use)
  - [Rules](#how-to-use---rules)
  - [AJAX validation](#how-to-use---ajax-validation)
  - [Advanced customisation](#how-to-use---advanced-customisation)
- [ASP.NET Core 3 Web API Sample](#web-api-sample)

## Purpose

If you want to use clientside validation with the FluentValidation library (you really should), then the recommended way is to use the [FluentValidation.AspNetCore](https://www.nuget.org/packages/FluentValidation.AspNetCore/) package with ASP.NET Core MVC. It integrates perfectly and there's no need for this filter at all.

However, _sometimes_ you may not be using MVC - you might be using an SPA application, such as React or Angular, or maybe a mobile application front-end. In this case you should probably want your clientside validators to match your server-side FluentValidtion validators, without having to duplicate the effort.

You still need to implement this in your front-end validation - this just provides a nice way to access the validator information for any endpoint, exposing and formatting the metadata provided by the FluentValidation library.

If there's any interest in this project/package I will be adding implementation examples (please contribute if you're able - particularly if you have an implementation with a specific front-end form library/generator).

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

For any given endpoint, add the query-string `?validation=1` to the endpoint URL in order to view the validator details, if applicable.

XML **is supported** but the documentation/examples will be using JSON (you can see a demonstration of both XML and JSON in the Web API sample project).

Example output for JSON:

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

> **Note:** You will need to put something in the body for POST/PUT requests, but if you add the `validation=1` query-string parameter then the posted body will be ignored - in the demo we simply post `{foo=bar}`.

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

You can validate any particular field by adding the query string _validate=fieldName_ and if the field is not valid, a problem details will be returned including errors only relevant to the specified field.

### How to use - Advanced customisation

You should find all customisation requirements are encapsulated in the `ValidationLister` class.

If you want to customise this package (such as the returned names for rules), I would advise that you copy/alter this class. Doing so will invalidate the unit tests but you can maintain those in your own fork.

Should you find a more "standard" way of presenting the validation information then please contribute to the project!

## Web API Sample

The included ASP.NET Core 3 Web API Sample application is designed to be a minimal demonstration of how this package works.

> There is no Razor/MVC example because this package is unnecessary for those projects. This package is for other front-ends that need access to the validation metadata.

Aside from the default Visual Studio template; it includes one controller, two models and two validators. The startup class includes the minimum setup detailed above.

The application runs a very basic (and fully-commented) snippet of JavaScript to return and display the validation metadata for an endpoint.

The validators in the project demonstrate these features of the filter;

- Child records are returned with a dot delimiter _(e.g. Address.Line1)_
- Custom regular expressions are returned _(e.g. Address.Postcode)_
- `.WithName()` is respected in the returned messages _(e.g. Address.Line1)_
- Custom error messages are returned where defined _(e.g. Address.County)_

> The sample application is designed for you to download and "prototype" or test this package. Feel free to contribute through pull-requests on GitHub!

There is a second page and JavaScript which tests the actual validation. It might be useful to test custom validators or other customisations.
