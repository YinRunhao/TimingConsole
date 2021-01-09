using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TimingConsole;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl+C to exit");
            // 不产生自身日志
            // ConsoleAppBuilder.CreateDefaultConsoleApp<DemoStartup>().Run();

            // 指定日志器记录自身日志
            ConsoleAppBuilder.CreateDefaultConsoleApp<DemoStartup>((sp)=> {
                return sp.GetService<ILogger>();
            }).Run();
        }
    }
}
