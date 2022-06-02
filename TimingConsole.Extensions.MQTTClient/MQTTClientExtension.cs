using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TimingConsole.Abstractions;

namespace TimingConsole.Extensions.MQTTClient
{
    public static class MQTTClientExtension
    {
        public static IServiceCollection AddMqttClient<T>(this IServiceCollection services, Action<MqttClientOptionsBuilder> optionBuilder)
            where T : class, IMqttClientConfiguration
        {
            services.AddSingleton<IMqttClientConfiguration, T>();

            services.AddConsoleService<MQTTClientService>(sp =>
            {
                var cfg = sp.GetRequiredService<IMqttClientConfiguration>();
                return new MQTTClientService(cfg, optionBuilder);
            });

            services.AddSingleton<IMqttClient>(sp =>
            {
                var svcLst = sp.GetServices<IConsoleService>();
                foreach(var svc in svcLst)
                {
                    if(svc is MQTTClientService mqtt)
                    {
                        return mqtt.Client;
                    }
                }

                return null;
            });

            return services;
        }
    }
}
