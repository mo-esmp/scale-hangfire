using Bogus;
using JobQueue.Shared;
using System;
using System.Collections.Generic;

namespace JobQueue.ProducerService
{
    internal class MessageGenerator
    {
        private static readonly string[] Categories = { "express", "normal" };
        private static readonly Faker<MessageModel> Faker;

        static MessageGenerator()
        {
            var random = new Random();

            Faker = new Faker<MessageModel>()
                .StrictMode(false)
                .RuleFor(p => p.Category, f => f.PickRandom(Categories))
                .RuleFor(p => p.MessageId, f => f.Random.Guid())
                .RuleFor(p => p.Entity, f => f.Finance.AccountName().Replace(" ", ""))
                .RuleFor(p => p.CreateDate, f => f.Date.Between(DateTime.Now.AddSeconds(-random.Next(1, 5)), DateTime.Now));
        }

        public static IEnumerable<MessageModel> GenerateMessages()
        {
            return Faker.Generate(100);
        }
    }
}