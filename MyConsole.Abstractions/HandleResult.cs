using System;

namespace MyConsole.Abstractions
{
    /// <summary>
    /// 处理执行结果
    /// </summary>
    public class HandleResult
    {
        /// <summary>
        /// 返回码，若命令类型小于零则退出
        /// </summary>
        public int Code;

        /// <summary>
        /// 打印消息
        /// </summary>
        public string Message;
    }
}
