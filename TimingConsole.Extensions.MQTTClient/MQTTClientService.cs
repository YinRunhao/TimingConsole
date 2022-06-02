using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimingConsole.Abstractions;

namespace TimingConsole.Extensions.MQTTClient
{
    /// <summary>
    /// MQTT客户端服务
    /// </summary>
    public class MQTTClientService : IConsoleService
    {
        public IMqttClient Client { get; private set; }

        private IMqttClientConfiguration m_Config;
        private Action<MqttClientOptionsBuilder> m_OptionConfig;

        public MQTTClientService(IMqttClientConfiguration config, Action<MqttClientOptionsBuilder> optionBuilder)
        {
            m_Config = config;
            m_OptionConfig = optionBuilder;
        }

        public async Task StartAsync()
        {
            Client = m_Config.CreateClient();
            m_Config.ConfigureClient(Client);
            MqttClientOptionsBuilder option = new MqttClientOptionsBuilder();
            m_OptionConfig(option);
            await Client.ConnectAsync(option.Build());
            await m_Config.ConfigureSubscribeAsync(Client);
        }

        public async Task StopAsync()
        {
            await Client.DisconnectAsync();
            Client.Dispose();
        }
    }
}
