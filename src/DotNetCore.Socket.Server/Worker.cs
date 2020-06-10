using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.Socket.Server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCore.Socket.Server
{
    public class Worker : BackgroundService
    {
        private const string SRV_START_MSG = "Starting Socket server...";
        private const string SRV_STOPPED_MSG = "Socket server stopped.";

        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            Console.WriteLine(SRV_START_MSG);
            this._logger.LogDebug(SRV_START_MSG);

            SocketServer.Start();

            while (!cancelToken.IsCancellationRequested && !SocketServer.IsClosed)
            {
                SocketServer.Listen();
            }

            SocketServer.Stop();
            Console.WriteLine(SRV_STOPPED_MSG);
            this._logger.LogDebug(SRV_STOPPED_MSG);

            await Task.CompletedTask;
        }
    }
}
