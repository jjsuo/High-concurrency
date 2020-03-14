using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCodes.Thread
{
    public static class ParallelHelper
    {
        /// <summary>
        /// 按指定的并行度，不间断从源中获取数据处理，直到源中数据走到最后
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="maxDegree"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task ForEach<T>(IAsyncEnumerable<T> source, int maxDegree, Func<T, Task> body)
        {
            var sourceEnumerator = source.GetAsyncEnumerator();

            SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

            List<Task> tasks = new List<Task>();

            for (var index = 0; index <= maxDegree - 1; index++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            T data = default(T);
                            await _lock.WaitAsync();
                            try
                            {
                                if (await sourceEnumerator.MoveNextAsync())
                                {
                                    data = sourceEnumerator.Current;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            finally
                            {
                                _lock.Release();
                            }

                            await body(sourceEnumerator.Current);
                        }
                    })
                    );

            }



            //等待最终所有任务完成
            foreach (var item in tasks)
            {
                await item;
            }

        }


    }
}
