using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 添加自定义控制台服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsoleService<T>(this IServiceCollection service)
            where T : class, IConsoleService
        {
            service.AddSingleton<IConsoleService, T>();
            return service;
        }

        /// <summary>
        /// 添加自定义控制台服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsoleService<T>(this IServiceCollection service, Func<IServiceProvider, T> factory)
            where T : class, IConsoleService
        {
            service.AddSingleton<IConsoleService, T>(factory);
            return service;
        }
    }
}
