using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 定时任务接口
    /// </summary>
    public interface ICron
    {
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <remarks>程序开始时执行一次，可进行定时任务的准备工作</remarks>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// 定时任务执行
        /// </summary>
        /// <remarks>每隔一段时间会以新对象执行该方法</remarks>
        /// <returns></returns>
        Task<HandleResult> ExecuteAsync();

        /// <summary>
        /// 程序结束前进行清理工作
        /// </summary>
        /// <remarks>程序结束时执行一次，可进行定时任务的清理工作</remarks>
        /// <returns></returns>
        Task ExitAsync();
    }
}
