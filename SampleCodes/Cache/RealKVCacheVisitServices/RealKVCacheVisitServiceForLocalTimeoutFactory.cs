using System;
using System.Collections.Generic;
using System.Text;

namespace SampleCodes.Cache.RealKVCacheVisitServices
{
    /// <summary>
    /// 单例工厂
    /// 简化DI处理
    /// </summary>
    public class RealKVCacheVisitServiceForLocalTimeoutFactory : SingletonFactorySelf<IRealKVCacheVisitService, RealKVCacheVisitServiceForLocalTimeoutFactory>
    {
        protected override IRealKVCacheVisitService RealCreate()
        {
            return new RealKVCacheVisitServiceForLocalTimeout();
        }
    }
}
