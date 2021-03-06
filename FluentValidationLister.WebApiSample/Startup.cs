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

    public sealed class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddXmlSerializerFormatters();

            services
                .AddFluentValidationLister()
                .AddTransient<IValidator<Person>, PersonValidator>()
                .AddTransient<IValidator<Address>, AddressValidator>();
        }

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
