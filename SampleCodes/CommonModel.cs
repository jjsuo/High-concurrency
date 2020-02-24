using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SampleCodes
{
    /// <summary>
    /// 通用DTO类
    /// 属性全部依赖Attributes
    /// 通常用于纯查询场景
    /// </summary>
    [DataContract]
    public class CommonModel : ModelBase
    {
    }
}
