﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;
using Samples.Shared;
using Samples.UsersApi.Database;

namespace Samples.UsersApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(opt => opt.UseSqlite("Data Source=Users.db"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var loggerFactory = new LoggerFactory().AddConsole();
            var logger = loggerFactory.CreateLogger<Program>();
            var tracer = JaegerTracer.CreateTracer("users-api", loggerFactory);
            services.AddOpenTracing();
            GlobalTracer.Register(tracer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
