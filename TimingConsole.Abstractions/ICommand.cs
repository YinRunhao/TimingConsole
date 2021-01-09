using System;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="param">命令行参数</param>
        /// <returns>执行结果</returns>
        HandleResult Execute(string[] param);
    }
}
