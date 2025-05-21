[![Build Status](https://github.com/Scandal-UK/FluentValidationLister/workflows/CI-CD%20Pipeline/badge.svg)](https://github.com/Scandal-UK/FluentValidationLister/actions?query=workflow%3A%22CI-CD%20Pipeline%22)
[![NuGet](https://img.shields.io/nuget/v/FluentValidationLister.Filter)](https://www.nuget.org/packages/FluentValidationLister.Filter)
[![Downloads](https://img.shields.io/nuget/dt/FluentValidationLister.Filter)](https://www.nuget.org/packages/FluentValidationLister.Filter)

# FluentValidationLister.Filter

**FluentValidationLister** is a .NET library that extracts and lists rules defined using [FluentValidation](https://github.com/FluentValidation/FluentValidation).

It provides introspectable metadata to the front-end that helps developers and client apps understand which validations/types apply to their models — useful for UI rendering, documentation, API consumers and client validators.

## 🚀 Installation

1. Install the package;
    ```bash
    dotnet add package FluentValidationLister
    ```

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

## 🧪 Try It Out

The middleware is invoked by appending ``?validation=1`` to any endpoint (simply use ``{}`` for POST/PUT requests). Explore the [FluentValidationLister.WebApiSample](https://github.com/Scandal-UK/FluentValidationLister/tree/main/FluentValidationLister.WebApiSample) to see the library in action. It also allows single-field validation by appending ``?validate={fieldName}`` to the URL.

The [sample](https://github.com/Scandal-UK/FluentValidationLister/tree/main/FluentValidationLister.WebApiSample) includes an ASP.NET Core API and a browser-based form that demonstrates how the validation rules can be consumed dynamically.
