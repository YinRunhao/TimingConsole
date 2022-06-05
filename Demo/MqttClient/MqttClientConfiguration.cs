using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimingConsole.Extensions.MQTTClient;

namespace Demo.MqttClient
{
    public class MqttClientConfiguration : IMqttClientConfiguration
    {
        public void ConfigureClient(IMqttClient clinet)
        {
            // 这里配置客户端的回调
            clinet.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(Recv);
        }

        public async Task ConfigureSubscribeAsync(IMqttClient client)
        {
            // 这里配置需要订阅的主题，连接后将第一时间订阅
            await client.SubscribeAsync("test");
        }

        public IMqttClient CreateClient()
        {
            // 创建客户端，该客户端将保持单例
            var ret = new MqttFactory().CreateMqttClient();
            return ret;
        }

        private void Recv(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ProcessingFailed == false)
            {
                string msg = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                PrintStr($"Client:{args.ClientId} send {msg}");
            }
            else
            {
                PrintStr($"Process failed");
            }
        }

        private void PrintStr(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
