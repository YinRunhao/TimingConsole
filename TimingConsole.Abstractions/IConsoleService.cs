using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 控制台服务
    /// </summary>
    public interface IConsoleService
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
    }
}
