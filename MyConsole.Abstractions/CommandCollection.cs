using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyConsole.Abstractions
{
    /// <summary>
    /// 控制台命令集合
    /// </summary>
    public class CommandCollection
    {
        private IServiceCollection m_Services;

        /// <summary>
        /// 内部字典
        /// </summary>
        protected Dictionary<string, Type> m_TypeDic;

        public CommandCollection(IServiceCollection services)
        {
            m_Services = services;
            m_TypeDic = new Dictionary<string, Type>();
        }

        public CommandCollection AddCommand<T>(string cmdNm) where T : class, ICommand
        {
            m_Services.AddScoped<T>();
            m_TypeDic.Add(cmdNm.ToLower(), typeof(T));
            return this;
        }

        public CommandCollection AddCommand<T>(string cmdNm, Func<IServiceProvider, T> fun) where T : class, ICommand
        {
            m_Services.AddScoped<T>(fun);
            m_TypeDic.Add(cmdNm.ToLower(), typeof(T));
            return this;
        }

        public Type this[string cmdNm]
        {
            get
            {
                if (m_TypeDic.TryGetValue(cmdNm.ToLower(), out Type ret))
                {
                    return ret;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
