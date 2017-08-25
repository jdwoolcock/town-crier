using System;
using System.Configuration;
using System.Timers;
using Topshelf;
using Topshelf.Logging;

namespace TownCrier
{
    class Program
    {
        public class ServiceSettings
        {
            public int IntervalMs { get; set; }
            public string Message { get; set; }
        }


        public class TownCrier
        {
            static readonly LogWriter log = HostLogger.Get<TownCrier>();

            readonly Timer timer;
            public TownCrier()
            {
                ServiceSettings settings = (dynamic)ConfigurationManager.GetSection("serviceSettings");
                var message = settings.Message;

                timer = new Timer(settings.IntervalMs) { AutoReset = true };
                timer.Elapsed += (sender, eventArgs) => log.Debug($"It is {DateTimeOffset.Now} and {message}");
            }
            public void Start() { timer.Start(); }
            public void Stop() { timer.Stop(); }
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            

            HostFactory.Run(x =>
            {
                

                

                x.Service<TownCrier>(s =>                           
                {
                    s.ConstructUsing(name => new TownCrier());      
                    s.WhenStarted(tc => tc.Start());                
                    s.WhenStopped(tc => tc.Stop());                 
                });
                x.RunAsLocalSystem();

                
                x.SetDescription("Town Crier");
                x.SetDisplayName("Town Crier");
                // You just have to make sure that service name in wix file exactly match service name in call to topshelf's SetServiceName().
                x.SetServiceName("Town Crier");
                x.UseLog4Net("log4net.config");
            });
            
            Console.Read();
        }
    }
}
