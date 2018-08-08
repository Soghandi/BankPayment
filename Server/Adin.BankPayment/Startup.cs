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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Adin.BankPayment.Util;
using Adin.BankPayment.TokenProvider;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;

namespace Adin.BankPayment
{
    public class Startup
    {
        private static readonly string _audience = "Adin";
        private static readonly string _issuer = "Soghandi";
        private static string SecretKey;
        private static SymmetricSecurityKey signingKey;


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthService(services);
            string connection = Configuration["ConnectionString"];
            services.AddDbContext<BankPaymentContext>(options => options.UseSqlServer(connection));

            try
            {
                // Register the Swagger generator, defining one or more Swagger documents
                services.AddSwaggerGen(options =>
                {

                    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Description = "BankPayment Api Guideline",
                        Title = "BankPayment API",
                        Version = "v1",
                        TermsOfService = "License Reserved - 2018",
                    });

                    options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
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

            SecretKey = Configuration.GetSection("SecretKey").Value;
            signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


            ConnectionStringGetter.ConStr = Configuration.GetSection("ConnectionString").Value;


            services.AddMemoryCache();
            services.AddMvc();
        }


        private void ConfigureAuthService(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.Audience = _audience;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = _issuer,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ClockSkew = TimeSpan.Zero,               
                };
            });         
        }

        private Task<ClaimsIdentity> GetIdentity(string publicKey)
        {
            UserValidator userValidator = new UserValidator();
            var res = userValidator.Validate(publicKey);
            if (res != null)
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(res.Id.ToString(), "Token"), new Claim[] { }));
            }
            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }


        private void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = _audience,
                Expiration = TimeSpan.FromHours(1),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            });
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
            app.UseAuthentication();
            ConfigureAuth(app);

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
