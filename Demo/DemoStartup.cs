using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimingConsole;
using TimingConsole.Abstractions;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    /// <summary>
    /// 示例启动配置
    /// </summary>
    public class DemoStartup : IAppStartup
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

            // 通过TimeSpan配置
            crons.AddCron<HelloCron>(TimeSpan.FromSeconds(interval));

            // 通过CRON表达式配置
            //crons.AddCron<HelloCron>("*/10 * * * * ?");
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
        }
    }
}
