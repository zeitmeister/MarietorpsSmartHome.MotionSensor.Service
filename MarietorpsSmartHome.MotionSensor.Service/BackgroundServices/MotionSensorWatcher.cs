using MarietorpsSmartHome.MotionSensor.Service.HelperFunctions;
using MarietorpsSmartHome.MotionSensor.Service.Models;
using MarietorpsSmartHome.MotionSensor.Service.Services;
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
        private readonly FortySixElksCred _fortySixElksCred;
        private readonly ICustomMqttClientFactory _mqttClientFactory;
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _mqttOptions;
        private List<ThresholdModel> ThresholdModels { get; set; } = new List<ThresholdModel>();
        public MotionSensorWatcher(IHelperFunctions helperFunctions, IOptionsMonitor<FortySixElksCred> optionsMonitorFortySixElksCred, ICustomMqttClientFactory mqttClientFactory)
        {
            _fortySixElksCred = optionsMonitorFortySixElksCred.CurrentValue;
            _helperFunctions = helperFunctions;
            _mqttClientFactory = mqttClientFactory;
            _mqttClient = _mqttClientFactory.MqttClient;
            _mqttOptions = _mqttClientFactory.MqttOptions;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
          
            Init();
            

            _mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                //_logger.LogInformation("### CONNECTED WITH SERVER ###");
                // Subscribe to a topic
                var result = await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("balcony/motion").Build());
                //_logger.LogInformation("### SUBSCRIBED ###");
                //Console.WriteLine("### SUBSCRIBED ###");
            });

            _mqttClient.UseApplicationMessageReceivedHandler(async e =>
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

            _mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                //_logger.LogError("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    //_logger.LogInformation("### ATTEMPTING TO RECONNECT TO THE SERVER ###");
                    await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                    //_logger.LogError("### RECONNECTING FAILED ###");
                }
                //_logger.LogInformation("### RECONNECTION SUCCESSFUL ###");
            });
            var connectResult = await _mqttClient.ConnectAsync(_mqttOptions);
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

        private void Init()
        {
                CheckConfig(_fortySixElksCred);
        }

        private static void CheckConfig(object classInstance)
        {
            
            var test = classInstance.GetType();
            foreach (var prop in test.GetProperties())
            {
                var value = (string)prop.GetValue(classInstance);
                if (string.IsNullOrEmpty(value)) 
                {
                    System.Console.WriteLine($"{prop.Name} of {test} is null. Please enter a value:");
                    
                    var temp = Console.ReadLine();
                    prop.SetValue(classInstance, temp);
                }
            } 
        }
    }
}
