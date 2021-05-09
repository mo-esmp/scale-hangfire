using JobQueue.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace JobQueue.ConsumerService.HttpClients
{
    public class JobHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<JobHttpClient> _logger;

        public JobHttpClient(HttpClient client, ILogger<JobHttpClient> logger)
        {
            _client = client;
            _logger = logger;
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<MessageModel>> GetJobMessagesAsync(CancellationToken token = default)
        {
            try
            {
                var response = await _client.GetAsync("api/v1/messages", token);
                var content = await response.Content.ReadAsStringAsync(token);
                var jobs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageModel>>(content);

                return jobs;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}