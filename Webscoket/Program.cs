using log4net;
using System;
using System.Configuration;
using Topshelf;
using Webscoket;

namespace WebScoket
{
    class Program
    {
        private static readonly ILog logger =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            HostFactory.Run(config =>
            {
                config.Service<SignalRServer>(instance =>
                {
                    instance.ConstructUsing(() =>
                        new SignalRServer(logger));

                    instance.WhenStarted(server => server.StartService());

                    instance.WhenStopped(server => server.StopService());
                });

                config.SetServiceName("WebScoket_Signal_server");
                config.SetDisplayName("WebSocket Signal server");
                config.SetDescription("Self hosted signal server using TopShelf for websocket to RTLS Dashboard");
                config.StartAutomatically();

                config.BeforeInstall(() =>
                {
                    logger.Info("Service before install");
                });

                config.AfterInstall(() =>
                {
                    logger.Info("Service after install");
                });

            });
        }
    }
}