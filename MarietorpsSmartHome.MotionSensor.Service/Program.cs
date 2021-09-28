using MarietorpsSmartHome.MotionSensor.Service.BackgroundServices;
using MarietorpsSmartHome.MotionSensor.Service.HelperFunctions;
using MarietorpsSmartHome.MotionSensor.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarietorpsSmartHome.MotionSensor.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    
                    services.AddHostedService<MotionSensorWatcher>();
                    services.AddHostedService<StoppingWatcher>();
                    services.AddSingleton<IHelperFunctions, HelperFunctions.HelperFunctions>();
                    services.AddSingleton<ICustomMqttClientFactory, MqttClientFactory>();
                    services.Configure<MQTTUser>(hostContext.Configuration.GetSection("MQTTUser"));
                    services.Configure<MQTTBroker>(hostContext.Configuration.GetSection("MQTTBroker"));
                    services.Configure<MQTTBroker>(hostContext.Configuration.GetSection("MQTTClient"));
                    services.Configure<FortySixElksCred>(hostContext.Configuration.GetSection("46ElksCreds"));

                });
    }
}
