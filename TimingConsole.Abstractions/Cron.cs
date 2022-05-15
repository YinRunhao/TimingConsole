using Cronos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TimingConsole.Abstractions
{
    public partial class CronCollection
    {
        /// <summary>
        /// 定时任务描述
        /// </summary>
        public class Cron
        {
            /// <summary>
            /// 执行间隔
            /// </summary>
            private TimeSpan m_Interval;

            /// <summary>
            /// 定时任务表达式
            /// </summary>
            private CronExpression m_CronExp;

            /// <summary>
            /// 定时任务逻辑类型
            /// </summary>
            public Type ExecType;

            /// <summary>
            /// 定时任务定时器
            /// </summary>
            public Timer Timer;

            public Cron(Type execType, TimeSpan interval)
            {
                ExecType = execType;
                m_Interval = interval;
            }

            public Cron(Type execType, string cronExp)
            {
                ExecType = execType;
                try
                {
                    m_CronExp = CronExpression.Parse(cronExp, CronFormat.Standard);
                }
                catch (FormatException)
                {
                    m_CronExp = CronExpression.Parse(cronExp, CronFormat.IncludeSeconds);
                }
            }

            /// <summary>
            /// 获取下次执行的时间间隔
            /// </summary>
            /// <returns></returns>
            public TimeSpan GetNextTimeSpan()
            {
                if (m_Interval == default)
                {
                    DateTime now = DateTime.UtcNow;
                    var next = m_CronExp.GetNextOccurrence(now, TimeZoneInfo.Local);
                    var nextTm = next.Value.ToLocalTime();
                    var ts = nextTm.Subtract(DateTime.Now);
                    return ts;
                }
                else
                {
                    return m_Interval;
                }
            }
        }
    }
}
