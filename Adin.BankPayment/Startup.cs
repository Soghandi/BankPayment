using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Cache;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Adin.BankPayment.Service;
using Adin.BankPayment.Domain.Model;


namespace Adin.BankPayment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            string connection = Configuration["ConnectionString"];
            services.AddDbContext<BankPaymentContext>(options => options.UseSqlServer(connection));

            try
            {
                // Register the Swagger generator, defining one or more Swagger documents
                services.AddSwaggerGen(options =>
                {
#if DEBUG
                    //options.IncludeXmlComments(@"SarayarApi.xml");
#else
                    // options.IncludeXmlComments(@"SarayarApi.xml");
#endif

                    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Description = "BankPayment Api Guideline",
                        Title = "BankPayment API",
                        Version = "v1",
                        TermsOfService = "License Reserved - 2018",
                    });

                    //     options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                });

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }


            services.AddDbContext<BankPaymentContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IRepository<Transaction>, TransactionRepository>();
            services.AddTransient<IRepository<Application>, ApplicationRepository>();
            services.AddTransient<IRepository<ApplicationBank>, ApplicationBankRepository>();
            services.AddTransient<IRepository<ApplicationBankParam>, ApplicationBankParamRepository>();
            services.AddTransient<IRepository<Bank>, BankRepository>();



            ConnectionStringGetter.ConStr = Configuration.GetSection("ConnectionString").Value;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            try
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api-docs";
                    c.RouteTemplate = "api-docs/{documentName}/bankpayment.json";
                });


                // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api-docs";
                    c.SwaggerEndpoint("/api-docs/v1/bankpayment.json", "BankPayment API V1");
                    //c.ShowRequestHeaders();
                });

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}
