using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarietorpsSmartHome.MotionSensor.Service.BackgroundServices
{
    public class StoppingWatcher : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Hello from PID {Process.GetCurrentProcess().Id}, press CTRL+C to exit.");

            AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).Unloading += context =>
            {
                Console.WriteLine("SIGTERM received, exiting program...");
            };

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("SIGINT received, exiting program...");
                Environment.Exit(0);
            };

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            finally
            {
                Console.WriteLine("Finally executed..");
            }
        }
    }
}
