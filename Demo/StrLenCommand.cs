using TimingConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    /// <summary>
    /// 示例命令，用于返回字符串长度
    /// </summary>
    public class StrLenCommand : ICommand
    {
        public HandleResult Execute(string[] param)
        {
            HandleResult result = new HandleResult();

            if(param.Length > 1)
            {
                result.Message = $"parameter error";
                result.Code = 2;
            }
            else
            {
                result.Message = $"string length:{param[0].Length}";
                result.Code = 1;
            }

            return result;
        }
    }
}
