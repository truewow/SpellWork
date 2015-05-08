using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using SpellWorkLib;

namespace SpellWorkWeb
{
    public static class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var port = 8080;
            if (args.Length > 0)
                int.TryParse(args[0], out port);

            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            new Loader();

            using (WebApp.Start<Startup>(string.Format("http://+:{0}", port)))
            {
                Console.WriteLine("Started");
                QuitEvent.WaitOne();
            }
        }
    }
}
