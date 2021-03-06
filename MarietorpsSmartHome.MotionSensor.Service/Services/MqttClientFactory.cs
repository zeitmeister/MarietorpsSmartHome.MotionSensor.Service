using MarietorpsSmartHome.MotionSensor.Service.Models;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MarietorpsSmartHome.MotionSensor.Service.Services
{
    public interface ICustomMqttClientFactory
    {
        IMqttClient MqttClient { get; }
        IMqttClientOptions MqttOptions { get; }
    }
    public class MqttClientFactory : ICustomMqttClientFactory
    {
        public IMqttClient MqttClient { get; set; }
        public IMqttClientOptions MqttOptions { get; set; }

        private readonly MQTTUser _mqttUser;
        private readonly MQTTBroker _mqttBroker;
        private readonly MQTTClient _mqttClient;

        public MqttClientFactory(IOptionsMonitor<MQTTUser> optionsMonitorMQTTUser, IOptionsMonitor<MQTTBroker> optionsMonitorMQTTBroker, IOptionsMonitor<MQTTClient> optionsMonitorMQTTClient)
        {
            _mqttUser = optionsMonitorMQTTUser.CurrentValue;
            _mqttBroker = optionsMonitorMQTTBroker.CurrentValue;
            _mqttClient = optionsMonitorMQTTClient.CurrentValue;

            MqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(_mqttClient.ClientName)
                .WithTcpServer(_mqttBroker.IpAddress, _mqttBroker.Port)
                .WithCredentials(_mqttUser.Username, _mqttUser.Password)
                .Build();
            MqttClient = new MqttFactory().CreateMqttClient();
        }
    }
}
