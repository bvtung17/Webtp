using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Webthucpham.Api;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalogs.Products;
using Webthucpham.ViewModels.System.Users;

namespace Webthucpham.AdminApp
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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = 2;
                options.ForwardedForHeaderName = "Header_Name_Used_By_Proxy_For_X-Forwarded-For_Header";
                options.ForwardedProtoHeaderName = "Header_Name_Used_By_Proxy_For_X-Forwarded-Proto_Header";
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(opts => opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Add session
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Http Client
            services.AddHttpClient();

            // Runtime compilation
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            // Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt =>
            {
                opt.LoginPath = "/Login/Index";
                opt.AccessDeniedPath = "/User/Forbidden/";
            });
            // Denpendency injections 
            services.AddTransient<IUserApiClient, UserApiClient>();
            //services.AddTransient<IRoleClientApi, RoleApiClient>();
            services.AddTransient<ICategoryApiClient, CategoryApiClient>();
            services.AddTransient<IProductApiClient, ProductApiClient>();
            services.AddTransient<IOrderApiClient, OrderApiClient>();
            //services.AddTransient<IBannerApiClient, BannerApiClient>();
            services.AddTransient<IClientApi, ClientApi>();
            services.AddTransient<IClientOrderApi, ClientOrderApi>();

            // Fluent Validation
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UpdateProductValidator>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProductImageCreateValidator>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProductImageUpdateValidator>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CategoryUpdateValidator>());
            //services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BannerCreateValidator>());
            //services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BannerUpdateValidator>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "product",
                    pattern: "{controller=Product}/{productId}/actions={images}"
                    );
            });
        }
    }
}
