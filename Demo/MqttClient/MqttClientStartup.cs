using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TimingConsole;
using TimingConsole.Abstractions;
using TimingConsole.Extensions.MQTTClient;

namespace Demo.MqttClient
{
    /// <summary>
    /// MQTT客户端示例启动配置
    /// </summary>
    public class MqttClientStartup : IAppStartup
    {
        public void ConfigureCommand(IConfiguration config, CommandCollection commands)
        {
            // 添加命令行交互处理对象
            commands.AddCommand<StrLenCommand>("strlen");
        }

        public void ConfigureCron(IConfiguration config, CronCollection crons)
        {
            // 添加定时任务处理对象
            // 读取配置
            int interval = config.GetValue<int>("HelloInterval");

            // 定时推送某些数据
            crons.AddCron<MqttPuhlishCron>(TimeSpan.FromSeconds(interval));
        }

        public void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            // 像ASP.net Core一样配置依赖注入
            services.AddSingleton<ILogger>((sp) =>
            {
                // 指定ILogger的依赖注入是NLog.config中配置好的AppLogger
                var factory = sp.GetService<ILoggerFactory>();
                return factory.CreateLogger("AppLogger");
            });
            services.AddLogging(loggingBuilder =>
            {
                // 使用NLog作为日志组件
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog();
            });

            // 配置MQTT客户端
            var mqttCfg = config.GetSection("MqttClient");
            string ip = mqttCfg.GetValue<string>("ServerIp");
            int port = mqttCfg.GetValue<int>("ServerPort");
            string clientId = mqttCfg.GetValue<string>("ClientId");

            services.AddMqttClient<MqttClientConfiguration>(option =>
            {
                option.WithTcpServer(ip, port)      // 服务器地址端口
                    .WithClientId(clientId)         // 客户端ID
                    .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311);
            });
        }
    }
}
