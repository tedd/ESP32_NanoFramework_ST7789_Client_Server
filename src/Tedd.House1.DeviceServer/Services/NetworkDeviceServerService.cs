using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tedd.House1.DeviceServer.Models;

namespace Tedd.House1.DeviceServer.Services
{
    class NetworkDeviceServerService: IHostedService
    {
        private readonly ILogger<NetworkDeviceServerService> _logger;
        private readonly AppSettings _settings;
        private NetworkDeviceServer _server;

        public NetworkDeviceServerService(ILogger<NetworkDeviceServerService> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public void Temp()
        {
            //Console.WriteLine("Starting server");
            //var server = new NetworkDeviceServer(4000);
            //var task = server.Start();
            //Console.ReadLine();
            //server.Stop();
            //Thread.Sleep(1000);

        }

        #region Implementation of IHostedService

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _server = new NetworkDeviceServer(_settings.NetworkDeviceServerListenPort);
            return _server.StartAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync();
        }

        #endregion
    }
}
