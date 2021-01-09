using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimingConsole.Abstractions;

namespace TimingConsole
{
    /// <summary>
    /// 默认控制台应用
    /// </summary>
    public class DefaultConsoleApp : ConsoleApp
    {
        /// <summary>
        /// 获取日志器的方法
        /// </summary>
        protected Func<IServiceProvider, ILogger> m_GetLogger;

        /// <summary>
        /// 启动项配置
        /// </summary>
        private IAppStartup m_Startup;

        public DefaultConsoleApp(IAppStartup startup)
        {
            m_Startup = startup;
            m_GetLogger = null;
            // 注册Ctrl + C键盘事件
            Console.CancelKeyPress += Console_CancelKeyPress;
            // 注册程序退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        public DefaultConsoleApp(IAppStartup startup, Func<IServiceProvider, ILogger> getLoggerFun) : this(startup)
        {
            m_GetLogger = getLoggerFun;
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

        protected override string Input()
        {
            return Console.ReadLine();
        }

        protected override void Output(string msg)
        {
            Console.WriteLine("> " + msg);
        }

        protected override ILogger GetLogger()
        {
            if(null == m_GetLogger)
            {
                return null;
            }
            else
            {
                return m_GetLogger(m_Service);
            }
        }
    }
}
