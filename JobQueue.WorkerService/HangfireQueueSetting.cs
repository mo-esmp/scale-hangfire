namespace JobQueue.WorkerService
{
    internal class HangfireQueueSetting
    {
        public string QueueName { get; set; }

        public int WorkerCount { get; set; }
    }
}