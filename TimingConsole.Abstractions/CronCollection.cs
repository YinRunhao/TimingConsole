using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TimingConsole.Abstractions
{
    /// <summary>
    /// 定时任务集合
    /// </summary>
    public partial class CronCollection : IEnumerable<CronCollection.Cron>
    {
        private IServiceCollection m_Services;

        /// <summary>
        /// 内部集合
        /// </summary>
        protected List<Cron> m_List;

        public CronCollection(IServiceCollection services)
        {
            m_Services = services;
            m_List = new List<Cron>();
        }

        public CronCollection AddCron<T>(TimeSpan span) where T : class, ICron
        {
            m_Services.AddScoped<T>();
            Cron c = new Cron(typeof(T), span);
            m_List.Add(c);
            return this;
        }

        public CronCollection AddCron<T>(TimeSpan span, Func<IServiceProvider, T> func) where T : class, ICron
        {
            m_Services.AddScoped<T>(func);
            Cron c = new Cron(typeof(T), span);
            m_List.Add(c);
            return this;
        }

        public CronCollection AddCron<T>(string cronExp) where T : class, ICron
        {
            m_Services.AddScoped<T>();
            Cron c = new Cron(typeof(T), cronExp);
            m_List.Add(c);
            return this;
        }

        public CronCollection AddCron<T>(string cronExp, Func<IServiceProvider, T> func) where T : class, ICron
        {
            m_Services.AddScoped<T>(func);
            Cron c = new Cron(typeof(T), cronExp);
            m_List.Add(c);
            return this;
        }

        public IEnumerator<Cron> GetEnumerator()
        {
            return ((IEnumerable<Cron>)m_List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_List).GetEnumerator();
        }
    }
}
