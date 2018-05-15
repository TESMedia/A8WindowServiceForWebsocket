using log4net;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscoket
{
   public class SignalRServer
    {
        private ILog logger;

        IDisposable SignalR { get; set; }

        public SignalRServer(ILog logger)
        {
            this.logger = logger;
        }

        public bool StartService()
        {
            logger.Info("Starting service...");
            var option = new StartOptions();
            option.Urls.Add(ConfigurationManager.AppSettings["HostUrl"].ToString());
            SignalR = WebApp.Start<StartUp>(option);
            logger.Info("SignalR server started..");
            logger.Info("Service Started.");
            return true;
        }

        public bool StopService()
        {
            SignalR.Dispose();
            logger.Info("Service Stopped.");
            System.Threading.Thread.Sleep(1500);
            return true;
        }

        public bool PauseService()
        {
            logger.Info("I'm in Pause method");
            return true;
        }
    }
}
