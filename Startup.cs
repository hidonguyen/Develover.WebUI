using Develover.WebUI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Develover.ERP.Web
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
            services.AddSession(cfg =>
            {                    // Đăng ký dịch vụ Session
                cfg.Cookie.Name = "Develover";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
                cfg.IdleTimeout = new TimeSpan(0, 60, 0);   // Thời gian tồn tại của Session
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new ViewBagFilter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Changed CultureInfo 1
            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var supportedCultures = new List<CultureInfo> { culture };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture, culture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/home/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();           // Đăng ký Middleware Session vào Pipeline

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapAreaControllerRoute(
                    name: "documents",
                    areaName: "documents",
                    pattern: "documents/{controller=home}/{action=index}");

                endpoints.MapAreaControllerRoute(
                    name: "stationery",
                    areaName: "stationery",
                    pattern: "stationery/{controller=home}/{action=index}");

                //endpoints.MapAreaControllerRoute(
                //    name: "vehicle",
                //    areaName: "vehicle",
                //    pattern: "vehicle/{controller=home}/{action=index}");


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}");
            });
        }
    }
}
