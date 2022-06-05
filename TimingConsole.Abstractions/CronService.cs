using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 定时任务服务
    /// </summary>
    public class CronService : IConsoleService
    {
        /// <summary>
        /// 容器
        /// </summary>
        private IServiceProvider m_Service;

        /// <summary>
        /// 宿主
        /// </summary>
        private ConsoleApp m_App;

        public CronService(IServiceProvider sp, ConsoleApp console)
        {
            m_Service = sp;
            m_App = console;
        }

        public async Task StartAsync()
        {
            ICron c = default;
            foreach (var cron in m_App.Crons)
            {
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        c = scope.ServiceProvider.GetRequiredService(cron.ExecType) as ICron;
                        await c.StartAsync();
                        if (c is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_App.GetLogger()?.LogError(ex, "CRON start error");
                        m_App.Output("CRON start error:" + ex.Message);
                    }
                }

                var ts = cron.GetNextTimeSpan();
                Timer timer = new Timer(ExecCron, cron, ts, TimeSpan.Zero);
                cron.Timer = timer;
            }
        }

        public async Task StopAsync()
        {
            int cronCnt = m_App.Crons.Count();
            int idx = 0;
            Task[] dispTks = new Task[cronCnt];
            // 关闭定时器
            foreach (var cron in m_App.Crons)
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
            foreach (var cron in m_App.Crons)
            {
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        c = scope.ServiceProvider.GetRequiredService(cron.ExecType) as ICron;
                        await c.ExitAsync();
                        if (c is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_App.GetLogger()?.LogError(ex, "CRON exit error");
                        m_App.Output("CRON exit error:" + ex.Message);
                    }
                }
            }
        }

        private void ExecCron(object state)
        {
            CronCollection.Cron self = state as CronCollection.Cron;
            Type tp = self.ExecType;
            ICron cron = default;
            if (tp != null)
            {
                string print = string.Empty;
                using (var scope = m_Service.CreateScope())
                {
                    try
                    {
                        cron = scope.ServiceProvider.GetRequiredService(tp) as ICron;
                        var result = cron.ExecuteAsync().GetAwaiter().GetResult();
                        print = result.Message;
                        if (cron is IDisposable dis)
                        {
                            dis.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_App.GetLogger()?.LogError(ex, "CRON execute error");
                        print = "CRON execute error:" + ex.Message;
                    }
                }
                m_App.Output(print);
            }

            var ts = self.GetNextTimeSpan();
            self.Timer.Change(ts, TimeSpan.Zero);
        }
    }
}
