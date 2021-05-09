using Hangfire;
using JobQueue.ConsumerService.HostedServices;
using JobQueue.ConsumerService.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace JobQueue.ConsumerService
{
    public class Startup
    {
        private static ConnectionMultiplexer Redis;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHostedService<MessageReceiverHostedService>();

            services.AddHttpClient<JobHttpClient>(options =>
            {
                Log.Logger.Warning("Producer service address: " + Configuration["JobApi:BaseAddress"]);
                options.BaseAddress = new Uri(Configuration["JobApi:BaseAddress"]);
                options.Timeout = TimeSpan.FromSeconds(5);
            }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(4)));

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(Redis));

            var queueSettings = Configuration.GetSection("Hangfire").Get<List<HangfireQueueSetting>>();
            foreach (var setting in queueSettings)
            {
                services.AddHangfireServer(options =>
                {
                    options.ServerName = $"{Environment.MachineName}:{setting.QueueName}";
                    options.Queues = new[] { setting.QueueName };
                    options.WorkerCount = setting.WorkerCount;
                });
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobQueue.ConsumerService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobQueue.ConsumerService v1"));
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });
            });
        }
    }

    public class HangfireQueueSetting
    {
        public string QueueName { get; set; }

        public int WorkerCount { get; set; }
    }
}