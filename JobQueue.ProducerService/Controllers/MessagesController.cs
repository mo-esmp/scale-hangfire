using JobQueue.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace JobQueue.ProducerService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessagesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<MessageModel> Get()
        {
            return MessageStore.Instance.GetMessages(new Random().Next(50, 200));
        }
    }
}