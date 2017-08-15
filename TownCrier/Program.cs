using System;
using System.Configuration;
using System.Timers;
using Topshelf;

namespace TestInstaller
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
            readonly Timer _timer;
            public TownCrier()
            {
                ServiceSettings settings = (dynamic)ConfigurationManager.GetSection("serviceSettings");
                var message = settings.Message;

                _timer = new Timer(settings.IntervalMs) { AutoReset = true };
                _timer.Elapsed += (sender, eventArgs) => Console.WriteLine($"It is {DateTimeOffset.Now} and {message}");
            }
            public void Start() { _timer.Start(); }
            public void Stop() { _timer.Stop(); }
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            

            HostFactory.Run(x =>                                    //1
            {
                x.Service<TownCrier>(s =>                           //2
                {
                    s.ConstructUsing(name => new TownCrier());      //3
                    s.WhenStarted(tc => tc.Start());                //4
                    s.WhenStopped(tc => tc.Stop());                 //5
                });
                x.RunAsLocalSystem();                               //6

                x.SetDescription("Sample Topshelf Host");           //7
                x.SetDisplayName("Stuff");                          //8
                x.SetServiceName("Stuff");                          //9
            });


            
            

            
            Console.ReadKey();
        }
    }
}
