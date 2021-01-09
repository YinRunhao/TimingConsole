using System;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 退出命令
    /// </summary>
    internal class ExitCommand : ICommand
    {
        public HandleResult Execute(string[] param)
        {
            HandleResult result = new HandleResult
            {
                Code = -1,
                Message = "Bye"
            };
            return result;
        }
    }
}
