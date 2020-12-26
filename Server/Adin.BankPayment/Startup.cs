using Adin.BankPayment.Domain.Cache;
using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Adin.BankPayment.Service;
using Adin.BankPayment.TokenProvider;
using Adin.BankPayment.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

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

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthService(services);
            var connection = Configuration["ConnectionString"];

            ConnectionStringGetter.ConStr = connection;

            services.AddDbContext<BankPaymentContext>(options => options.UseSqlServer(ConnectionStringGetter.ConStr, b => b.MigrationsAssembly("Adin.BankPayment")));

            try
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Description = "BankPayment Api Guideline",
                        Title = "BankPayment API",
                        Version = "v1",
                        TermsOfService = new Uri("https://github.com/Soghandi/BankPayment/blob/master/LICENSE")
                    });

                    options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            services.AddTransient<IRepository<Transaction>, TransactionRepository>();
            services.AddTransient<IRepository<Application>, ApplicationRepository>();
            services.AddTransient<IRepository<ApplicationBank>, ApplicationBankRepository>();
            services.AddTransient<IRepository<ApplicationBankParam>, ApplicationBankParamRepository>();
            services.AddTransient<IRepository<Bank>, BankRepository>();

            services.AddHttpClient();

            SecretKey = Configuration.GetSection("SecretKey").Value;
            signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            services.AddMemoryCache();
            services.AddMvc(e => e.EnableEndpointRouting = false);
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
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        private Task<ClaimsIdentity> GetIdentity(string publicKey)
        {
            var userValidator = new UserValidator();
            var res = userValidator.Validate(publicKey);
            if (res != null)
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(res.Id.ToString(), "Token"),
                    Array.Empty<Claim>()));
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
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
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            try
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api-help";
                    c.RouteTemplate = "api-help/{documentName}/bankpayment.json";
                });


                // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api-help";
                    c.SwaggerEndpoint("/api-help/v1/bankpayment.json", "BankPayment API V1");
                    //c.ShowRequestHeaders();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}