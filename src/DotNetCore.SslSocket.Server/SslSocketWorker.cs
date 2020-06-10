using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.SslSocket.Server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCore.SslSocket.Server
{
    public class SslSocketWorker : BackgroundService
    {
        private const string SRV_START_MSG = "Starting Socket server...";
        private const string SRV_STOPPED_MSG = "Socket server stopped.";

        private readonly ILogger<SslSocketWorker> _logger;

        public SslSocketWorker(ILogger<SslSocketWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            Console.WriteLine(SRV_START_MSG);
            this._logger.LogDebug(SRV_START_MSG);

            SslSocketServer.Start();

            while (!cancelToken.IsCancellationRequested)
            {
                SslSocketServer.Listen();
            }

            SslSocketServer.Stop();
            Console.WriteLine(SRV_STOPPED_MSG);
            this._logger.LogDebug(SRV_STOPPED_MSG);

            await Task.CompletedTask;
        }
    }
}
