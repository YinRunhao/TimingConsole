using System;
using System.Collections.Generic;
using System.Text;

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
            public TimeSpan Interval;

            /// <summary>
            /// 定时任务逻辑类型
            /// </summary>
            public Type ExecType;
        }
    }
}
