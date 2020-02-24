using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleCodes.Cache.RealKVCacheVisitServices.KVCacheVersionServices
{
    /// <summary>
    /// 用来测试的KV缓存版本服务
    /// 10秒换一次版本号
    /// </summary>
    public class KVCacheVersionServiceForTest : IKVCacheVersionService
    {
        private static DateTime? _tiem;
        private static string _version;
        public async Task<string> GetVersion(string versionName)
        {
            return await Task.FromResult(GetVersionSync(versionName));
        }

        public string GetVersionSync(string versionName)
        {
            if (!_tiem.HasValue || (DateTime.UtcNow - _tiem.Value).TotalSeconds >= 10)
            {
                _tiem = DateTime.UtcNow;
                _version = Guid.NewGuid().ToString();
            }

            return _version;
        }
    }
}
