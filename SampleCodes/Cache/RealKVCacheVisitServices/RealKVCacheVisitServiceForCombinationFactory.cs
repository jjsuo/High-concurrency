using System;
using System.Collections.Generic;
using System.Text;

namespace SampleCodes.Cache.RealKVCacheVisitServices
{
    public class RealKVCacheVisitServiceForCombinationFactory : SingletonFactorySelf<IRealKVCacheVisitService, RealKVCacheVisitServiceForCombinationFactory>
    {

        protected override IRealKVCacheVisitService RealCreate()
        {
            var kvCacheVisitorRepository = KVCacheVisitorRepositoryFactory.Get();
            RealKVCacheVisitServiceForCombination realKVCacheVisitServiceForCombination = new RealKVCacheVisitServiceForCombination(kvCacheVisitorRepository);
            return realKVCacheVisitServiceForCombination;
        }
    }
}
