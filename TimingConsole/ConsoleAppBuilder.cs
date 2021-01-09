using Microsoft.Extensions.Logging;
using TimingConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole
{
    public static class ConsoleAppBuilder
    {
        /// <summary>
        /// 创建控制台应用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ConsoleApp Create<T>(params object[] args) where T : ConsoleApp
        {
            return Activator.CreateInstance(typeof(T), args) as ConsoleApp;
        }

        /// <summary>
        /// 以指定的启动项配置获取默认控制台应用
        /// </summary>
        /// <remarks>自身不产生日志</remarks>
        /// <typeparam name="T">启动项配置</typeparam>
        /// <returns>默认控制台应用</returns>
        public static ConsoleApp CreateDefaultConsoleApp<T>() where T : IAppStartup
        {
            IAppStartup startup = Activator.CreateInstance(typeof(T)) as IAppStartup;
            return Create<DefaultConsoleApp>(startup);
        }

        /// <summary>
        /// 以指定的启动项配置获取默认控制台应用，并以指定的日志器记录自身日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getLoggerFun">自身日志器获取方法</param>
        /// <returns></returns>
        public static ConsoleApp CreateDefaultConsoleApp<T>(Func<IServiceProvider, ILogger> getLoggerFun)
        {
            IAppStartup startup = Activator.CreateInstance(typeof(T)) as IAppStartup;
            return Create<DefaultConsoleApp>(startup, getLoggerFun);
        }
    }
}
