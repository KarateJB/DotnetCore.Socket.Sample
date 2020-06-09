using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCore.Socket.Server
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            Console.WriteLine("Starting Socket server...");

            while (!cancelToken.IsCancellationRequested)
            {
            }

            Console.WriteLine("Socket server stoped.");
        }
    }
}
