using System;
using System.Threading;
using MyConsole;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl+C to exit");
            ConsoleAppBuilder.CreateDefaultConsoleApp<DemoStartup>().Run();
        }
    }
}
