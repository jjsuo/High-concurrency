using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleCodes.Cache
{
    public class KVCacheVisitorRepository : IKVCacheVisitorRepository
    {
        /// <summary>
        /// 为了简化操作，这里直接使用一个静态变量来存储数据
        /// 实际场景中，数据可能需要来源在持久化层
        /// </summary>
        public static Dictionary<string, KVCacheVisitor> Datas { get; } = new Dictionary<string, KVCacheVisitor>();

        public async Task<KVCacheVisitor> QueryByName(string name)
        {
            return await Task.FromResult(QueryByNameSync(name));
        }

        public KVCacheVisitor QueryByNameSync(string name)
        {
            return Datas[name];
        }
    }

    public class KVCacheVisitorRepositoryFactory : SingletonFactorySelf<IKVCacheVisitorRepository, KVCacheVisitorRepositoryFactory>
    {
        protected override IKVCacheVisitorRepository RealCreate()
        {
            return new KVCacheVisitorRepository();
        }
    }
}
