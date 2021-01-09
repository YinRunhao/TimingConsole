using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimingConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole
{
    /// <summary>
    /// 控制台应用启动项接口
    /// </summary>
    public interface IAppStartup
    {
        /// <summary>
        /// 配置命令行处理对象
        /// </summary>
        /// <param name="commands">命令处理对象集合</param>
        /// <param name="config">配置文件</param>
        void ConfigureCommand(IConfiguration config, CommandCollection commands);

        /// <summary>
        /// 配置定时任务
        /// </summary>
        /// <param name="crons">定时任务集合</param>
        /// <param name="config">配置文件</param>
        void ConfigureCron(IConfiguration config, CronCollection crons);

        /// <summary>
        /// 配置依赖注入
        /// </summary>
        /// <param name="services">容器集合</param>
        void ConfigureServices(IConfiguration config, IServiceCollection services);
    }
}
