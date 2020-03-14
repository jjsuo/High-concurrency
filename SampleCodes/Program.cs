using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SampleCodes.Cache;
using SampleCodes.Cache.RealKVCacheVisitServices;
using SampleCodes.Cache.RealKVCacheVisitServices.KVCacheVersionServices;
using SampleCodes.Thread;
using SampleCodes.Collections;
using System.Diagnostics.Tracing;

namespace SampleCodes
{
    class Program
    {
        async static Task Main(string[] args)
        {



            AsyncInteration<string> asyncInteration = new AsyncInteration<string>(
                async (index) =>
                {
                    if (index <= 3)
                    {
                        //模拟数据源
                        return await Task.FromResult(new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "A", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });
                    }

                    return null;
                }
                );


            var pollingResult= await PollingHelper.Polling<string>(
                async()=>
                {
                    return await Task.FromResult(
                    new AsyncInteration<string>(
                        async (index) =>
                        {
                            if (index <= 3)
                            {
                                //模拟数据源
                                return await Task.FromResult(new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "A", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });
                            }

                            return null;
                        }
                        )
                    );
                },3,1000,
                async (data) =>
                {
                    //模拟耗时操作
                    await Task.Delay(100);

                    Console.WriteLine(data);
                }
                ,
                async(ex)=>
                {
                    Console.WriteLine(ex.ToString());
                    await Task.CompletedTask;
                }
                );


            
            //10秒后，关闭轮询
            await Task.Delay(10000);
            await pollingResult.Stop();


            Console.ReadLine();
            /*await ParallelHelper.ForEach(asyncInteration, 3, async (data) =>
            {
                //模拟耗时操作
                await Task.Delay(300);

            Console.WriteLine(data);
        });*/



            //初始化
            //Init();


            //await LocalTimeout();

            //await LocalVersion();

            //await Combination();

        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            KVCacheVisitorIMP.RealKVCacheVisitServiceFactories["LocalTimeout"] = RealKVCacheVisitServiceForLocalTimeoutFactory.GetFactory();
            KVCacheVisitorIMP.RealKVCacheVisitServiceFactories["LocalVersion"] = RealKVCacheVisitServiceForLocalVersionFactory.GetFactory();
            KVCacheVisitorIMP.RealKVCacheVisitServiceFactories["Combination"] = RealKVCacheVisitServiceForCombinationFactory.GetFactory();

            RealKVCacheVisitServiceForLocalVersion.KVCacheVersionServiceFactories["Test"] = KVCacheVersionServiceForTestFactory.GetFactory();

            KVCacheVisitorRepository.Datas["Cache1"]= new KVCacheVisitor()
            {
                Name = "Cache1",
                CacheType = "LocalTimeout",
                CacheConfiguration = @"{
                                        ""MaxLength"":2,
                                        ""ExpireSeconds"":-1
                                       }"
            };

            KVCacheVisitorRepository.Datas["Cache2"] = new KVCacheVisitor()
            {
                Name = "Cache2",
                CacheType = "LocalVersion",
                CacheConfiguration = @"{
                                        ""MaxLength"":3,
                                        ""VersionCallTimeout"":5,
                                        ""VersionNameMappings"":
                                            {
                                                ""System.String-System.String"":""Test""
                                            },
                                         ""DefaultVersionName"":""Test""
                                       }"
            };

            KVCacheVisitorRepository.Datas["Cache3"] = new KVCacheVisitor()
            {
                Name = "Cache3",
                CacheType = "Combination",
                CacheConfiguration = @"{
                                        ""VistorNames"":[""Cache1"",""Cache2""]
                                       }"
            };


        }

        private static async Task LocalTimeout()
        {

            var cache=await KVCacheVisitorRepositoryFactory.Get().QueryByName("Cache1");

            for(var index = 0;index<=15;index++)
            {
                //创建key1，key2,key3,三个缓存
                var cacheValue = await cache.Get<string, string>(
                     async (k) =>
                     {
                         return await Task.FromResult(Guid.NewGuid().ToString());
                     }, "Key1");

                Console.WriteLine($"Key1:{cacheValue}");

                cacheValue = await cache.Get<string, string>(
                                async (k) =>
                                {
                                    return await Task.FromResult(Guid.NewGuid().ToString());
                                }, "Key2");

                Console.WriteLine($"Key2:{cacheValue}");

                cacheValue = await cache.Get<string, string>(
                async (k) =>
                {
                    return await Task.FromResult(Guid.NewGuid().ToString());
                }, "Key3");

                Console.WriteLine($"Key3:{cacheValue}");


                await Task.Delay(300);
            }
        }

        private static async Task LocalVersion()
        {
            var cache = await KVCacheVisitorRepositoryFactory.Get().QueryByName("Cache2");

            for (var index = 0; index <= 15; index++)
            {
                //创建key1，key2,key3,三个缓存
                var cacheValue = await cache.Get<string, string>(
                     async (k) =>
                     {
                         return await Task.FromResult(Guid.NewGuid().ToString());
                     }, "Key1");

                Console.WriteLine($"Key1:{cacheValue}");

                cacheValue = await cache.Get<string, string>(
                                async (k) =>
                                {
                                    return await Task.FromResult(Guid.NewGuid().ToString());
                                }, "Key2");

                Console.WriteLine($"Key2:{cacheValue}");

                cacheValue = await cache.Get<string, string>(
                async (k) =>
                {
                    return await Task.FromResult(Guid.NewGuid().ToString());
                }, "Key3");

                Console.WriteLine($"Key3:{cacheValue}");


                await Task.Delay(100);
            }


        }

        private static async Task Combination()
        {
            var cache = await KVCacheVisitorRepositoryFactory.Get().QueryByName("Cache3");


             for (var index = 0; index <= 15; index++)
             {
                 //创建key1，key2,key3,三个缓存
                 var cacheValue = await cache.Get<string, string>(
                      async (k) =>
                      {
                          return await Task.FromResult(Guid.NewGuid().ToString());
                      }, "Key1");

                 Console.WriteLine($"Key1:{cacheValue}");

                 cacheValue = await cache.Get<string, string>(
                                 async (k) =>
                                 {
                                     return await Task.FromResult(Guid.NewGuid().ToString());
                                 }, "Key2");

                 Console.WriteLine($"Key2:{cacheValue}");

                 cacheValue = await cache.Get<string, string>(
                 async (k) =>
                 {
                     return await Task.FromResult(Guid.NewGuid().ToString());
                 }, "Key3");

                 Console.WriteLine($"Key3:{cacheValue}");


                 await Task.Delay(100);
             }

     
        }

    }
}
