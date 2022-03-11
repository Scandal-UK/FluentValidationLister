// <copyright file="Program.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace FluentValidationLister.WebApiSample
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary> Entry point of application. </summary>
    public static class Program
    {
        /// <summary>Method representing main program loop.</summary>
        /// <param name="args">Application parameter arguments.</param>
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        /// <summary> Method to define and return a host builder using the <see cref="Startup"/> class. </summary>
        /// <param name="args">Application parameter arguments.</param>
        /// <returns>Instance of <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
