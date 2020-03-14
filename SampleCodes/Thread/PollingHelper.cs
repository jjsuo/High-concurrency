using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCodes.Thread
{
    public static class PollingHelper
    {
        /// <summary>
        /// 按指定的并行度，不间断从源中获取数据处理，当每次数据源取完后，wait指定的interval毫秒数，再次轮询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="maxDegree"></param>
        /// <param name="interval"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<IAsyncPollingResult> Polling<T>(Func<Task<IAsyncEnumerable<T>>> sourceGereratorFun, int maxDegree, int interval, Func<T, Task> body, Func<Exception, Task> exceptionHandler)
        {
            SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

            var sourceEnumerator = (await sourceGereratorFun()).GetAsyncEnumerator();

            List<Task> tasks = new List<Task>();

            AsyncPollingResultDefault result = new AsyncPollingResultDefault(
                async () =>
                {
                    //等待最终所有任务完成
                    foreach (var item in tasks)
                    {
                        await item;
                    }
                }
                );


            for (var index = 0; index <= maxDegree - 1; index++)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            if (result.IsStop)
                            {
                                break;
                            }
                            var tempSourceEnumerator = sourceEnumerator;


                            await _lock.WaitAsync();
                            T data = default(T);
                            bool isError = false;
                            try
                            {
                                if (await sourceEnumerator.MoveNextAsync())
                                {
                                    data = sourceEnumerator.Current;
                                }
                                else
                                {
                                    await Task.Delay(interval);

                                    if (tempSourceEnumerator != sourceEnumerator)
                                    {
                                        continue;
                                    }
                                    sourceEnumerator = (await sourceGereratorFun()).GetAsyncEnumerator();
                                }
                            }
                            catch (Exception ex)
                            {
                                isError = true;
                                await exceptionHandler(ex);
                            }
                            finally
                            {
                                _lock.Release();
                            }

                            if (!isError)
                            {
                                try
                                {
                                    await body(data);
                                }
                                catch (Exception ex)
                                {
                                    await exceptionHandler(ex);
                                }
                            }

                        }
                    })
                    );

            }

            return result;

        }

    }

    public interface IAsyncPollingResult
    {
        Task Stop();
    }

    public class AsyncPollingResultDefault : IAsyncPollingResult
    {
        private bool _stop = false;
        private Func<Task> _completeAction;

        public AsyncPollingResultDefault(Func<Task> completeAction)
        {
            _completeAction = completeAction;
        }

        public async Task Stop()
        {
            _stop = true;
            if (_completeAction != null)
            {
                await _completeAction();
            }
            await Task.CompletedTask;
        }

        public bool IsStop
        {
            get
            {
                return _stop;
            }
        }
    }

}
