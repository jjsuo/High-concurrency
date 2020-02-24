using System;
using System.Collections.Generic;
using System.Text;

namespace SampleCodes
{
    /// <summary>
    /// 单例自助工厂
    /// 仅为减少代码
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public abstract class SingletonFactorySelf<T, R> : SingletonFactory<T>
        where T : class
        where R : IFactory<T>
    {
        private static R _factory;
        static SingletonFactorySelf()
        {
            _factory = (R)typeof(R).Assembly.CreateInstance(typeof(R).FullName);
        }

        public static T Get()
        {
            return _factory.Create();
        }
        public static IFactory<T> GetFactory()
        {
            return _factory;
        }
    }
}
