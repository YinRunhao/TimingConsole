# MyConsole

一个基于.Net Standard 2.1的控制台程序模板框架，可在其轻松添加定时任务和命令行交互。使用了.Net标准的依赖注入扩展、日志扩展和配置文件扩展，能像在ASP.net Core中一样轻松获取对象和配置程序。

## 使用方法

如Demo项目中的例子所示。

### 1.定义自己的命令行处理类和定时任务处理类

命令行处理类需实现**ICommand**的接口

```c#
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
```



定时任务处理类需要实现**ICron**接口

```c#
    /// <summary>
    /// 示例定时任务，定时向控制台打印Hello World
    /// </summary>
    public class HelloCron : ICron
    {
        public async Task<HandleResult> Execute()
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

        public Task Exit()
        {
            return Task.CompletedTask;
        }

        public Task Start()
        {
            return Task.CompletedTask;
        }
    }
```

### 2.编写程序启动配置

程序启动配置类实现**IAppStarup**接口，在其接口方法中添加命令行处理类、定时任务类和依赖注入。

```C#
    /// <summary>
    /// 示例启动配置
    /// </summary>
    public class DemoStartup : IAppStartup
    {
        public void ConfigureCommand(IConfiguration config, CommandCollection commands)
        {
            // 添加命令行交互处理对象
            commands.AddCommand<StrLenCommand>("strlen");
        }

        public void ConfigureCron(IConfiguration config, CronCollection crons)
        {
            // 添加定时任务处理对象
            // 读取配置
            int interval = config.GetValue<int>("HelloInterval");
            crons.AddCron<HelloCron>(TimeSpan.FromSeconds(interval));
        }

        public void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            // 像ASP.net Core一样配置依赖注入
            services.AddSingleton<ILogger>((sp) =>
            {
                // 指定ILogger的依赖注入是NLog.config中配置好的AppLogger
                var factory = sp.GetService<ILoggerFactory>();
                return factory.CreateLogger("AppLogger");
            });
            services.AddLogging(loggingBuilder =>
            {
                // 使用NLog作为日志组件
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog();
            });
        }
    }
```

### 3.开始运行

以指定的启动项配置获取默认控制台应用并运行。

```
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl+C to exit");
            ConsoleAppBuilder.CreateDefaultConsoleApp<DemoStartup>().Run();
        }
    }
```

### 不使用默认实现

可以自行继承**MyConsole.Abstractions**里面的**ConsoleApp**抽象类来实现更多自定义功能。

### 命令行使用示例

> commands.AddCommand<StrLenCommand>("strlen");

如上所示，在启动配置中添加了StrLenCommand命令，命令的名称是**strlen**
在控制台中输入："strlen helloworld"即可以helloworld作为参数调用StrLenCommand的Execute方法。
控制台输入字符串解析规则： {命令名称} {参数1} {参数2} ...


## 注意事项

1. 程序默认读取的配置文件是根目录下的**appsettings.json**文件。
2. 程序启动后调用完定时任务的Start方法后不会立即执行定时任务，而是5秒后开始。
3. 每次定时任务的执行和命令的执行获取的依赖注入范围都是独立的Scope，可以放心地将需要依赖注入的类型设定为AddScoped。
4. 若定时任务和命令行执行类实现了IDisposable接口，则完成后会自动调用其Dispose方法。
5. 命令行字符串以空格分隔参数。
6. 程序退出时会等待所有定时任务执行完最后一次才退出。

