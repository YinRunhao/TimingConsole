using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimingConsole.Abstractions;

namespace Demo.MqttClient
{
    /// <summary>
    /// 定时推送示例
    /// </summary>
    public class MqttPuhlishCron : ICron
    {
        private IMqttClient m_Client;

        public MqttPuhlishCron(IMqttClient client)
        {
            m_Client = client;
        }

        public async Task<HandleResult> ExecuteAsync()
        {
            HandleResult ret = new HandleResult();
            try
            {
                await Send("Hello world");
                ret.Code = 1;
                ret.Message = "Send Ok";
            }
            catch (Exception ex)
            {
                ret.Code = 0;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task StartAsync()
        {
            await Send("爷来了");
        }

        public Task ExitAsync()
        {
            // 不需要手动关闭MQTT连接，MQTT连接在程序退出时将自动关闭
            return Task.CompletedTask;
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task Send(string msg)
        {
            await m_Client.PublishAsync(new MqttApplicationMessage
            {
                Topic = "test",
                QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce,
                Payload = Encoding.UTF8.GetBytes(msg)
            });
        }
    }
}
