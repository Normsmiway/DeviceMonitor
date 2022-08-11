using BlazorAnimate;
using Blazored.Modal;
using DeviceMonitorApp.Code;
using DeviceMonitorApp.Code.Notifications;
using DeviceMonitorApp.Data;
using DeviceMonitorApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealDB.Lib;
using System;
using System.Linq;
using System.Net.Http;
using TableDependency.SqlClient.Base;

namespace DeviceMonitorApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();
            services.ConfigureNotification(Configuration);
            // Server Side Blazor doesn't register HttpClient by default
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                // Setup HttpClient for server side in a client side compatible fashion
                services.AddScoped(s =>
                {
                    // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.      
                    var uriHelper = s.GetRequiredService<NavigationManager>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.BaseUri)
                    };
                });
            }
          
            services.Configure<AnimateOptions>("down", options =>
            {
                options.Animation = Animations.SlideUp;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });

            services.Configure<AnimateOptions>("up", options =>
            {
                options.Animation = Animations.SlideDown;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });
            services.Configure<AnimateOptions>("test", options =>
            {
                options.Animation = Animations.ZoomIn;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });

            services.Configure<AnimateOptions>("left", options =>
            {
                options.Animation = Animations.SlideLeft;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });

            services.Configure<AnimateOptions>("right", options =>
            {
                options.Animation = Animations.SlideRight;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });
            services.Configure<AnimateOptions>("device", options =>
            {
                options.Animation = Animations.FadeDown;
                options.Easing = Easings.EaseIn;
                options.Duration = TimeSpan.FromSeconds(2);
            });

            services.Configure<AnimateOptions>("chat", options =>
             {
                 options.Animation = Animations.FadeDown;
                 options.Duration = TimeSpan.FromMilliseconds(100);
             });

            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<IDeviceMockService, MockDeviceService>();
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<INotifier, DeviceStatusNotifier>();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }


    }

    public static class Extension
    {
        public static void ConfigureNotification(this IServiceCollection services, IConfiguration Configuration)
        {
            #region improved as seen below
            //services.AddTransient(typeof(RecordStatusNotifier<DeviceModel>), c =>
            //{
            //    var mapper = new ModelToTableMapper<DeviceModel>();
            //    mapper.AddMapping(c => c.Id, "activityid");
            //    mapper.AddMapping(c => c.Name, "keys");
            //    mapper.AddMapping(c => c.Status, "status");
            //    mapper.AddMapping(c => c.CashLevel, "cashlevel");
            //    mapper.AddMapping(c => c.Cassette, "Cassette");
            //    mapper.AddMapping(c => c.CardReader, "CardReader");
            //    mapper.AddMapping(c => c.CashJam, "CashJam");
            //    mapper.AddMapping(c => c.NetworkStatus, "networkstatus");
            //    mapper.AddMapping(c => c.ReceiptPrinter, "ReceiptPrinter");
            //    mapper.AddMapping(c => c.ReceiptPaper, "ReceiptPaper");
            //    return new RecordStatusNotifier<DeviceModel>(Configuration, mapper);
            //});
            #endregion
            
            var TableName = Configuration["AppSettings:TableName"] ?? throw new ArgumentNullException("Table Name");
            var ConnectionString = Configuration["ConnectionStrings:Default"] ?? throw new ArgumentNullException("Connection string");


            services.AddTransient(typeof(RecordStatusNotifier<DeviceModel>), c =>
            {
                var mapper = new ModelToTableMapper<DeviceModel>();

                mapper.AddMapping(c => c.Id, "activityid");
                mapper.AddMapping(c => c.Name, "keys");
                mapper.AddMapping(c => c.Status, "status");
                mapper.AddMapping(c => c.CashLevel, "cashlevel");
                mapper.AddMapping(c => c.Cassette, "Cassette");
                mapper.AddMapping(c => c.CardReader, "CardReader");
                mapper.AddMapping(c => c.CashJam, "CashJam");
                mapper.AddMapping(c => c.NetworkStatus, "networkstatus");
                mapper.AddMapping(c => c.ReceiptPrinter, "ReceiptPrinter");
                mapper.AddMapping(c => c.ReceiptPaper, "ReceiptPaper");

                //  var mappings = mapper.GetMappings();
                return new RecordStatusNotifier<DeviceModel>(ConnectionString, TableName, mapper);
            });
        }
    }
}
