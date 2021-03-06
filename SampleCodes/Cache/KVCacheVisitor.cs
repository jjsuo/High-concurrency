﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace SampleCodes.Cache
{
    public class KVCacheVisitor : EntityBase<IKVCacheVisitorIMP>
    {
        private static IFactory<IKVCacheVisitorIMP> _kvCacheVisitorIMPFactory= new KVCacheVisitorIMPFactory();

        public static IFactory<IKVCacheVisitorIMP> KVCacheVisitorIMPFactory
        {
            set
            {
                _kvCacheVisitorIMPFactory = value;
            }
        }
        public override IFactory<IKVCacheVisitorIMP> GetIMPFactory()
        {
            return _kvCacheVisitorIMPFactory;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid ID
        {
            get
            {
                return GetAttribute<Guid>("ID");
            }
            set
            {
                SetAttribute<Guid>("ID", value);
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return GetAttribute<string>("Name");
            }
            set
            {
                SetAttribute<string>("Name", value);
            }
        }

        /// <summary>
        /// 缓存类型
        /// </summary>
        public string CacheType
        {
            get
            {
                return GetAttribute<string>("CacheType");
            }
            set
            {
                SetAttribute<string>("CacheType", value);
            }
        }

        /// <summary>
        /// 缓存配置
        /// </summary>
        public string CacheConfiguration
        {
            get
            {
                return GetAttribute<string>("CacheConfiguration");
            }
            set
            {
                SetAttribute<string>("CacheConfiguration", value);
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return GetAttribute<DateTime>("CreateTime");
            }
            set
            {
                SetAttribute<DateTime>("CreateTime", value);
            }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime
        {
            get
            {
                return GetAttribute<DateTime>("ModifyTime");
            }
            set
            {
                SetAttribute<DateTime>("ModifyTime", value);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="creator">如果缓存不存在，需要创建数据的动作</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<V> Get<K, V>(Func<K, Task<V>> creator, K key)
        {
            return await _imp.Get<K, V>(this, creator, key);
        }


        /// <summary>
        /// 获取数据（同步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="creator">如果缓存不存在，需要创建数据的动作</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public V GetSync<K, V>(Func<K, V> creator, K key)
        {
            return _imp.GetSync<K, V>(this, creator, key);
        }
    }

    public interface IKVCacheVisitorIMP
    {
        Task<V> Get<K, V>(KVCacheVisitor visito, Func<K, Task<V>> creator, K key);
        V GetSync<K, V>(KVCacheVisitor visitor, Func<K, V> creator, K key);
    }


    public class KVCacheVisitorIMP : IKVCacheVisitorIMP
    {
        private static Dictionary<string, IFactory<IRealKVCacheVisitService>> _realKVCacheVisitServiceFactories = new Dictionary<string, IFactory<IRealKVCacheVisitService>>();

        /// <summary>
        /// 实际KV缓存访问服务工厂键值对
        /// 键为缓存类型
        /// </summary>
        public static IDictionary<string, IFactory<IRealKVCacheVisitService>> RealKVCacheVisitServiceFactories
        {
            get
            {
                return _realKVCacheVisitServiceFactories;
            }
        }

        public async Task<V> Get<K, V>(KVCacheVisitor visitor, Func<K, Task<V>> creator, K key)
        {
            var prefix = GetPrefix(visitor.Name, typeof(K), typeof(V));
            var realService = getRealService(visitor.CacheType);

            return await realService.Get(visitor.CacheConfiguration,
                async () =>
                {
                    return await creator(key);
                }
            , prefix, key);

        }

        public V GetSync<K, V>(KVCacheVisitor visitor, Func<K, V> creator, K key)
        {
            var prefix = GetPrefix(visitor.Name, typeof(K), typeof(V));
            var realService = getRealService(visitor.CacheType);

            return realService.GetSync(visitor.CacheConfiguration,
                 () =>
                 {
                     return creator(key);
                 }
            , prefix, key);
        }

        private string GetPrefix(string name, Type keyType, Type valueType)
        {
            return $"{name}_{keyType.FullName}_{valueType.FullName}";
        }

        public IRealKVCacheVisitService getRealService(string type)
        {
            if (!_realKVCacheVisitServiceFactories.TryGetValue(type, out IFactory<IRealKVCacheVisitService> serviceFactory))
            {

                //简化处理，直接抛出Exception
                throw new Exception($"找不到缓存类型为{type}的实际KV缓存访问服务");
            }

            return serviceFactory.Create();

        }
    }

    public class KVCacheVisitorIMPFactory : IFactory<IKVCacheVisitorIMP>
    {
        public IKVCacheVisitorIMP Create()
        {
            return new KVCacheVisitorIMP();
        }
    }

    public interface IRealKVCacheVisitService
    {
        Task<V> Get<K, V>(string cacheConfiguration, Func<Task<V>> creator, string prefix, K key);
        V GetSync<K, V>(string cacheConfiguration, Func<V> creator, string prefix, K key);
    }
}
