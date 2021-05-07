using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobQueue.ProducerService.HostedServices
{
    public class MessageProducerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SeedData, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void SeedData(object state)
        {
            if (MessageStore.Instance.Count > 2000)
                return;

            var messages = MessageGenerator.GenerateMessages();
            MessageStore.Instance.AddMessages(messages);
        }
    }
}