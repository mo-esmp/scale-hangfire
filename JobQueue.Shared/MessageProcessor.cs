using Hangfire;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace JobQueue.Shared
{
    public class MessageProcessor
    {
        [Queue("express")]
        [DisplayName("JobId: {1}")]
        [AutomaticRetry(Attempts = 3)]
        public static async Task ProcessExpressMessageAsync(MessageModel message, Guid messageId)
        {
            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 4)));
        }

        [Queue("normal")]
        [DisplayName("JobId: {1}")]
        [AutomaticRetry(Attempts = 3)]
        public static async Task ProcessNormalMessageAsync(MessageModel message, Guid messageId)
        {
            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 3)));
        }
    }
}