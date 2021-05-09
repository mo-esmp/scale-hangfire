using Hangfire;
using Hangfire.States;
using JobQueue.ConsumerService.HttpClients;
using JobQueue.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace JobQueue.ConsumerService.HostedServices
{
    public class MessageReceiverHostedService : IHostedService
    {
        private readonly CancellationTokenSource _cts;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageReceiverHostedService> _logger;

        public MessageReceiverHostedService(IServiceProvider serviceProvider, ILogger<MessageReceiverHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _cts = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => FetchMessagesAsync(_cts.Token), cancellationToken);

            _logger.LogInformation("Service started");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service is stopping");
            _cts.Cancel();

            return Task.CompletedTask;
        }

        private async Task FetchMessagesAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                using var scope = _serviceProvider.CreateScope();
                var httpClient = scope.ServiceProvider.GetRequiredService<JobHttpClient>();
                var messages = await httpClient.GetJobMessagesAsync(cancellationToken);

                if (!messages.Any())
                    continue;

                var categories = messages.GroupBy(m => m.Category).ToList();

                Parallel.ForEach(categories, category =>
                {
                    Enqueue(category.Key, category.ToList());
                });

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        private void Enqueue(string queueName, List<MessageModel> messages)
        {
            var client = new BackgroundJobClient();
            var state = new EnqueuedState(queueName);

            foreach (var message in messages.OrderBy(o => o.CreateDate))
            {
                Expression<Action> action = queueName == "express"
                    ? () => MessageProcessor.ProcessExpressMessageAsync(message, message.MessageId)
                    : () => MessageProcessor.ProcessNormalMessageAsync(message, message.MessageId);
                client.Create(action, state);
            }
        }
    }
}