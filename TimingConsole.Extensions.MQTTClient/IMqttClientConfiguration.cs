using System;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace TimingConsole.Extensions.MQTTClient
{
    /// <summary>
    /// MQTT客户端配置
    /// </summary>
    public interface IMqttClientConfiguration
    {
        /// <summary>
        /// 创建MQTTnet客户端
        /// </summary>
        /// <returns></returns>
        IMqttClient CreateClient();

        /// <summary>
        /// 配置客户端的回调等
        /// </summary>
        /// <param name="clinet"></param>
        void ConfigureClient(IMqttClient clinet);

        /// <summary>
        /// 配置客户端的订阅主题
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task ConfigureSubscribeAsync(IMqttClient client);
    }
}
