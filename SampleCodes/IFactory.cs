using System;
using System.Collections.Generic;
using System.Text;

namespace SampleCodes
{
    /// <summary>
    /// 工厂接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<out T>
    {
        T Create();
    }
}
