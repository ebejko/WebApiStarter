using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApiStarter.Constants;
using WebApiStarter.Data;
using WebApiStarter.Filters;

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
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services
                .AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
                .AddBearerToken(options =>
                {
                    options.BearerTokenExpiration = Configuration.GetValue<TimeSpan?>("BearerToken:BearerTokenExpiration") ?? options.BearerTokenExpiration;
                    options.RefreshTokenExpiration = Configuration.GetValue<TimeSpan?>("BearerToken:RefreshTokenExpiration") ?? options.RefreshTokenExpiration;
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

                c.AddSecurityDefinition(BearerTokenDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = AuthenticationConstants.SchemeName,
                    Name = AuthenticationConstants.HeaderName,
                    Description = AuthenticationConstants.Description
                });

                c.OperationFilter<AuthResponsesOperationFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlPath);
            });
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
