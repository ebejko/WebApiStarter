using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebApiStarter.Constants;
using WebApiStarter.Data;
using WebApiStarter.Filters;
using WebApiStarter.Services;

namespace WebApiStarter
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
            
            services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Token:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["Token:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SigningKey"])),
                        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:EncryptionKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers(options => 
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.OutputFormatters.RemoveType<StreamOutputFormatter>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Configuration["Api:Version"],
                    new OpenApiInfo
                    {
                        Title = Configuration["Api:Name"],
                        Description = Configuration["Api:Description"],
                        Version = Configuration["Api:Version"]
                    });

                c.AddSecurityDefinition(AuthenticationConstants.SchemeName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Name = AuthenticationConstants.HeaderName,
                    BearerFormat = AuthenticationConstants.BearerFormat,
                    Description = AuthenticationConstants.Description,
                });

                c.OperationFilter<AuthResponsesOperationFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddScoped<ITokenService, TokenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/internal/exception");
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger(c => 
            {
                c.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = Configuration["Api:Name"];
                c.SwaggerEndpoint($"/api-docs/{Configuration["Api:Version"]}/swagger.json", $"{Configuration["Api:Name"]} {Configuration["Api:Version"]}");
                c.RoutePrefix = String.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Default", "Internal");
            });
        }
    }
}
