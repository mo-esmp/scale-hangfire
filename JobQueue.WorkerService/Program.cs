using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace JobQueue.WorkerService
{
    public class Program
    {
        private static ConnectionMultiplexer _redis;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    _redis = ConnectionMultiplexer.Connect(hostContext.Configuration.GetConnectionString("RedisConnection"));

                    services.AddHangfire(configuration => configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseRedisStorage(_redis));

                    var queueSettings = hostContext.Configuration.GetSection("Hangfire").Get<List<HangfireQueueSetting>>();
                    foreach (var setting in queueSettings)
                    {
                        services.AddHangfireServer(options =>
                        {
                            options.ServerName = $"{Environment.MachineName}:{setting.QueueName}";
                            options.Queues = new[] { setting.QueueName };
                            options.WorkerCount = setting.WorkerCount;
                        });
                    }
                });
    }
}