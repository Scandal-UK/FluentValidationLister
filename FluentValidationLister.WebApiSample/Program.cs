// <copyright file="Program.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

using FluentValidation;
using FluentValidationLister.Filter.DependencyInjection;
using FluentValidationLister.WebApiSample.Models;
using FluentValidationLister.WebApiSample.Models.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddFluentValidationLister()
    .AddTransient<IValidator<Person>, PersonValidator>()
    .AddTransient<IValidator<Address>, AddressValidator>()
    .AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
