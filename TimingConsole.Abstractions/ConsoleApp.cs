using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 控制台程序基类
    /// </summary>
    public abstract class ConsoleApp
    {
        /// <summary>
        /// 依赖注入集合
        /// </summary>
        protected IServiceProvider m_Service;

        /// <summary>
        /// 命令集合
        /// </summary>
        protected CommandCollection m_Commands;

        /// <summary>
        /// 定时任务集合
        /// </summary>
        protected CronCollection m_Crons;

        /// <summary>
        /// 定时任务定时器集合
        /// </summary>
        //private List<Timer> m_CronTimers;

        /// <summary>
        /// 配置
        /// </summary>
        private IConfiguration m_Configuration;

        /// <summary>
        /// 是否退出标记
        /// </summary>
        private byte m_Exit;

        public ConsoleApp()
        {
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        public void Run()
        {
            Initialize();
            m_Exit = 0;
            CronRun();
            string cmdStr = string.Empty;
            string cmdNm = string.Empty;
            HandleResult result;
            Type cmdTp = default;
            ICommand cmd = default;
            while (m_Exit == 0)
            {
                cmdStr = Input();
                if(cmdStr == null)
                {
                    cmdStr = string.Empty;
                    Exit();
                }
                if (string.IsNullOrWhiteSpace(cmdStr))
                {
                    continue;
                }
                var param = cmdStr.Split(' ');
                if (param.Length > 0)
                {
                    var cmdParam = GetCommandParam(param);
                    cmdNm = cmdParam.cmdNm;
                    cmdTp = m_Commands[cmdNm];
                    if (cmdTp == null)
                    {
                        Output($"Command [{cmdNm.Trim()}] not found");
                        continue;
                    }

                    using (var scope = m_Service.CreateScope())
                    {
                        try
                        {
                            cmd = scope.ServiceProvider.GetRequiredService(cmdTp) as ICommand;
                            // 执行
                            result = cmd.Execute(cmdParam.cmdParam);
                            if (result.Code < 0)
                            {
                                // 返回码小于0就退出
                                Exit();
                            }
                            Output(result.Message);

                            if (cmd is IDisposable dsp)
                            {
                                dsp.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            GetLogger()?.LogError(ex, "Command exec error");
                            Output("Command execute error:" + ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 暂停已配置的定时任务
        /// </summary>
        /// <exception cref="ArgumentException">定时任务类型未配置</exception>
        /// <typeparam name="T">已配置的定时任务类型</typeparam>
        public void CronPause<T>()
        {
            CronPause(typeof(T));
        }

        /// <summary>
        /// 暂停已配置的定时任务
        /// </summary>
        /// <exception cref="ArgumentException">定时任务类型未配置</exception>
        /// <param name="t">已配置的定时任务类型</param>
        public void CronPause(Type t)
        {
            CronCollection.Cron c = default;
            foreach(var cron in m_Crons)
            {
                if(cron.ExecType == t)
                {
                    c = cron;
                    break;
                }
            }
            if (c != null)
            {
                // 停止定时器
                c.Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
            else
            {
                throw new ArgumentException($"Cron [{t.Name}] not found");
            }
        }

        /// <summary>
        /// 开始已暂停的定时任务
        /// </summary>
        /// <exception cref="ArgumentException">定时任务类型未配置</exception>
        /// <typeparam name="T">已配置的定时任务类型</typeparam>
        public void CronStart<T>()
        {
            CronStart(typeof(T));
        }

        /// <summary>
        /// 开始已暂停的定时任务
        /// </summary>
        /// <exception cref="ArgumentException">定时任务类型未配置</exception>
        /// <param name="t">已配置的定时任务类型</param>
        public void CronStart(Type t)
        {
            CronCollection.Cron c = default;
            foreach (var cron in m_Crons)
            {
                if (cron.ExecType == t)
                {
                    c = cron;
                    break;
                }
            }
            if (c != null)
            {
                // 开始定时器
                c.Timer.Change(TimeSpan.Zero, c.Interval);
            }
            else
            {
                throw new ArgumentException($"Cron [{t.Name}] not found");
            }
        }

        /// <summary>
        /// 程序退出，无论何种实现，程序退出前请务必调用此方法
        /// </summary>
        protected void Exit()
        {
            if(m_Exit == 0)
            {
                m_Exit = 1;
                // 关闭定时器
                CronEnd();
            }
        }

        /// <summary>
        /// 控制台打印输出
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void Output(string msg);

        /// <summary>
        /// 控制台输入
        /// </summary>
        /// <remarks>若要退出程序请返回null</remarks>
        /// <returns></returns>
        protected abstract string Input();

        /// <summary>
        /// 获取主模块的日志器，该日志器用于记录本类库产生的日志
        /// </summary>
        /// <remarks>可根据业务情景自行实现，若不希望本类库产生日志请返回null</remarks>
        /// <returns></returns>
        protected virtual ILogger GetLogger()
        {
            return null;
            //return m_Service.GetService<ILogger>();
        }

        /// <summary>
        /// 设置应用的配置文件
        /// </summary>
        /// <remarks>默认读取根目录的appsettings.json</remarks>
        /// <returns></returns>
        protected virtual IConfigurationBuilder ConfigAppConfiguration()
        {
            var cfgBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            return cfgBuilder;
        }

        /// <summary>
        /// 配置依赖注入
        /// </summary>
        /// <param name="services">容器集合</param>
        protected abstract void ConfigureServices(IConfiguration config, IServiceCollection services);

        /// <summary>
        /// 配置命令行处理对象
        /// </summary>
        /// <param name="commands">命令处理对象集合</param>
        /// <param name="config">配置文件</param>
        protected abstract void ConfigureCommand(IConfiguration config, CommandCollection commands);

        /// <summary>
        /// 配置定时任务
        /// </summary>
        /// <param name="crons">定时任务集合</param>
        /// <param name="config">配置文件</param>
        protected abstract void ConfigureCron(IConfiguration config, CronCollection crons);

        private async void CronRun()
        {
            ICron c = default;
            foreach (var cron in m_Crons)
            {
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        c = scope.ServiceProvider.GetRequiredService(cron.ExecType) as ICron;
                        await c.Start();
                        if (c is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLogger()?.LogError(ex, "CRON start error");
                        Output("CRON start error:" + ex.Message);
                    }
                }

                Timer timer = new Timer(ExecCron, cron.ExecType, TimeSpan.FromSeconds(5), cron.Interval);
                //m_CronTimers.Add(timer);
                cron.Timer = timer;
            }
        }

        private void ExecCron(object state)
        {
            Type tp = state as Type;
            ICron cron = default;
            if (tp != null)
            {
                string print = string.Empty;
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        cron = scope.ServiceProvider.GetRequiredService(tp) as ICron;
                        var result = cron.Execute().GetAwaiter().GetResult();
                        print = result.Message;
                        if (cron is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLogger()?.LogError(ex, "CRON execute error");
                        print = "CRON execute error:" + ex.Message;
                    }
                }
                Output(print);
            }
        }

        private void CronEnd()
        {
            int cronCnt = m_Crons.Count();
            int idx = 0;
            Task[] dispTks = new Task[cronCnt];
            // 关闭定时器
            foreach(var cron in m_Crons)
            {
                var timer = cron.Timer;
                // 等待正在执行的任务完成
                dispTks[idx] = Task.Run(async () =>
                {
                    await timer.DisposeAsync();
                });
                idx++;
            }
            Task.WaitAll(dispTks);

            // 调用定时任务的结束方法
            ICron c;
            foreach (var cron in m_Crons)
            {
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        c = scope.ServiceProvider.GetRequiredService(cron.ExecType) as ICron;
                        c.Exit().GetAwaiter().GetResult();
                        if (c is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLogger()?.LogError(ex, "CRON exit error");
                        Output("CRON exit error:" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化启动配置
        /// </summary>
        private void Initialize()
        {
            var serviceArr = new ServiceCollection();
            SetConfiguration(serviceArr);
            // 将自己加入依赖注入
            serviceArr.AddSingleton<ConsoleApp>((sp) => { return this; });
            ConfigureServices(m_Configuration, serviceArr);

            var cmds = new CommandCollection(serviceArr);
            var crons = new CronCollection(serviceArr);

            cmds.AddCommand<ExitCommand>("exit");

            ConfigureCommand(m_Configuration, cmds);
            ConfigureCron(m_Configuration, crons);
            m_Service = serviceArr.BuildServiceProvider();

            m_Commands = cmds;
            m_Crons = crons;
        }

        private void SetConfiguration(IServiceCollection services)
        {
            var cfgBuilder = ConfigAppConfiguration();
            m_Configuration = cfgBuilder.Build();

            services.AddSingleton<IConfiguration>((sp) =>
            {
                return m_Configuration;
            });
        }

        /// <summary>
        /// 处理控制台参数
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>命令名+执行参数</returns>
        private (string cmdNm, string[] cmdParam) GetCommandParam(string[] args)
        {
            // 命令名
            string cmdNm = args[0];

            // 拼参数
            string[] execParam = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                execParam[i - 1] = args[i];
            }
            return (cmdNm, execParam);
        }
    }
}
