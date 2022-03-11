// <copyright file="Startup.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample
{
    using FluentValidation;
    using FluentValidationLister.Filter.DependencyInjection;
    using FluentValidationLister.WebApiSample.Models;
    using FluentValidationLister.WebApiSample.Models.Validators;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary> Standard Startup class. </summary>
    public sealed class Startup
    {
        /// <summary> Configures the services in the container. </summary>
        /// <param name="services">Service collection.</param>
        public static void ConfigureServices(IServiceCollection services) =>
            services
                .AddFluentValidationLister()
                .AddTransient<IValidator<Person>, PersonValidator>()
                .AddTransient<IValidator<Address>, AddressValidator>()
                .AddControllers();

        /// <summary> Configures the application. </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Host environment.</param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseStaticFiles();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
