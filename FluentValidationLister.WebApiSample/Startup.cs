namespace FluentValidationLister.WebApiSample
{
    using FluentValidation;
    using FluentValidationLister.Filter.DependencyInjection;
    using FluentValidationLister.WebApiSample.Models;
    using FluentValidationLister.WebApiSample.Models.Validators;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            })
                .AddXmlSerializerFormatters();

            services
                .AddFluentValidationLister()
                .AddTransient<IValidator<Person>, PersonValidator>()
                .AddTransient<IValidator<Address>, AddressValidator>()
                .AddTransient<IValidator<Country>, CountryValidator>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
