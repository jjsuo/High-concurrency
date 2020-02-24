using System;
using System.Collections.Generic;
using System.Text;

namespace SampleCodes.Cache.RealKVCacheVisitServices.KVCacheVersionServices
{
    /// <summary>
    /// 单例工厂
    /// 简化DI处理
    /// </summary>
    public class KVCacheVersionServiceForTestFactory : SingletonFactorySelf<IKVCacheVersionService, KVCacheVersionServiceForTestFactory>
    {
        protected override IKVCacheVersionService RealCreate()
        {
            return new KVCacheVersionServiceForTest();
        }
    }
}
