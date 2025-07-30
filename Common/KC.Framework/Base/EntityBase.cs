using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Framework.Base
{
    /// <summary>
    ///     可持久到数据库的领域模型的基类。
    /// </summary>
    [Serializable]
    [ProtoContract]
    [DataContract(IsReference = true)]
    public abstract class EntityBase
    {
        
    }
}
