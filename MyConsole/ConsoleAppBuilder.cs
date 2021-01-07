using MyConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyConsole
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
        /// <typeparam name="T">启动项配置</typeparam>
        /// <returns>默认控制台应用</returns>
        public static ConsoleApp CreateDefaultConsoleApp<T>() where T : IAppStartup
        {
            IAppStartup startup = Activator.CreateInstance(typeof(T)) as IAppStartup;
            return Create<DefaultConsoleApp>(startup);
        }
    }
}
