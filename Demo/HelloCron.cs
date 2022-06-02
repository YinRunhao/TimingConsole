using TimingConsole.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet;

namespace Demo
{
    /// <summary>
    /// 示例定时任务，定时向控制台打印Hello World
    /// </summary>
    public class HelloCron : ICron
    {
        public async Task<HandleResult> ExecuteAsync()
        {
            HandleResult result = new HandleResult();
            // 假装工作了100ms
            await Task.Delay(100);
            // 输出信息
            result.Message = "Hello World";
            // 返回码，小于0退出程序
            result.Code = 1;

            return result;
        }

        public Task ExitAsync()
        {
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }
    }
}
