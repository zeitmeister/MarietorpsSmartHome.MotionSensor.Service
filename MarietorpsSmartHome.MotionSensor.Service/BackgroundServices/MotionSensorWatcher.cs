using MarietorpsSmartHome.MotionSensor.Service.HelperFunctions;
using MarietorpsSmartHome.MotionSensor.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarietorpsSmartHome.MotionSensor.Service.BackgroundServices
{
    public class MotionSensorWatcher : BackgroundService
    {
        private readonly IHelperFunctions _helperFunctions;
        private readonly IConfiguration _configuration;
        private readonly MQTTUser _mqttUser;
        private readonly MQTTBroker _mqttBroker;
        private readonly MQTTClient _mqttClient;
        private readonly FortySixElksCred _fortySixElksCred;
        private List<ThresholdModel> ThresholdModels { get; set; } = new List<ThresholdModel>();
        public MotionSensorWatcher(IHelperFunctions helperFunctions, IOptionsMonitor<MQTTUser> optionsMonitorMQTTUser, IOptionsMonitor<MQTTBroker> optionsMonitorMQTTBroker, IOptionsMonitor<MQTTClient> optionsMonitorMQTTClient, IOptionsMonitor<FortySixElksCred> optionsMonitorFortySixElksCred)
        {
            _fortySixElksCred = optionsMonitorFortySixElksCred.CurrentValue;
            _mqttUser = optionsMonitorMQTTUser.CurrentValue;
            _mqttBroker = optionsMonitorMQTTBroker.CurrentValue;
            _mqttClient = optionsMonitorMQTTClient.CurrentValue;
            _helperFunctions = helperFunctions;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(_mqttClient.ClientName)
                .WithTcpServer(_mqttBroker.IpAddress, _mqttBroker.Port)
                .WithCredentials(_mqttUser.Username, _mqttUser.Password)
                .Build();
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                //_logger.LogInformation("### CONNECTED WITH SERVER ###");
                // Subscribe to a topic
                var result = await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("balcony/motion").Build());
                //_logger.LogInformation("### SUBSCRIBED ###");
                //Console.WriteLine("### SUBSCRIBED ###");
            });

            mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                Console.WriteLine("Message recieved");
                ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
                if (_helperFunctions.ThresholdIsReached(ThresholdModels))
                {
                    //await SendSms(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    await SendSms("Motion detected");
                    ThresholdModels.Clear();
                }
                Console.WriteLine(ThresholdModels.Count);
                if (ThresholdModels.Count >= 3)
                {
                    ThresholdModels.Clear();
                }
                
            });

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                //_logger.LogError("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    //_logger.LogInformation("### ATTEMPTING TO RECONNECT TO THE SERVER ###");
                    await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                    //_logger.LogError("### RECONNECTING FAILED ###");
                }
                //_logger.LogInformation("### RECONNECTION SUCCESSFUL ###");
            });
            var connectResult = await mqttClient.ConnectAsync(mqttOptions);
            Console.WriteLine("Connected?!");
        }

        private async Task SendSms(string v)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.46elks.com");

            client.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                  System.Text.ASCIIEncoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", _fortySixElksCred.ClientId, _fortySixElksCred.ClientSecret))));

            var content = new FormUrlEncodedContent(new[] {
            new KeyValuePair < string, string > ("from", "MTSH"),
            new KeyValuePair < string, string > (
              "to", _fortySixElksCred.PhoneNumber),
            new KeyValuePair < string, string > (
              "message",
              $"{v} at {DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")}")
          });

            HttpResponseMessage response = await client
              .PostAsync("/a1/SMS", content);
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
        }
    }
}
