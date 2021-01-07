using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyConsole.Abstractions;

namespace MyConsole
{
    /// <summary>
    /// 默认控制台应用
    /// </summary>
    public class DefaultConsoleApp : ConsoleApp
    {
        private IAppStartup m_Startup;

        public DefaultConsoleApp(IAppStartup startup)
        {
            m_Startup = startup;
            // 注册Ctrl + C键盘事件
            Console.CancelKeyPress += Console_CancelKeyPress;
            // 注册程序退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Exit();
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Exit();
            e.Cancel = true;
        }

        protected override void ConfigureCommand(IConfiguration config, CommandCollection commands)
        {
            m_Startup.ConfigureCommand(config, commands);
        }

        protected override void ConfigureCron(IConfiguration config, CronCollection crons)
        {
            m_Startup.ConfigureCron(config, crons);
        }

        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            m_Startup.ConfigureServices(config, services);
        }
    }
}
