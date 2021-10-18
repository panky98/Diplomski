using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace StreamingMicroservice
{
    public class StreamingHub : Hub
    {
        public StreamingHub(ILogger<StreamingHub> logger)
        {
            _logger = logger;
        }
        private readonly ILogger<StreamingHub> _logger;
        public async Task AddToGroup(string code)//groupName would be code for event
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, code);
        }

        public async Task SendByte(IAsyncEnumerable<byte> list, string code)
        {
            await foreach(var line in list)
            {
                _logger.LogInformation("SENDING BYTE!");
                await Clients.Group(code).SendAsync("ReceiveByte", line);
            }
        }
    }
}
