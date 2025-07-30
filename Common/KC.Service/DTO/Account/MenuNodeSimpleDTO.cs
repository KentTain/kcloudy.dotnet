using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class MenuNodeSimpleDTO : TreeNodeSimpleDTO<MenuNodeSimpleDTO>, IEqualityComparer<MenuNodeSimpleDTO>, ICloneable
    {
        public MenuNodeSimpleDTO()
        {
        }

        /// <summary>
        /// 请求地址
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [ProtoMember(1)]
        public string AreaName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(128)]
        [ProtoMember(2)]
        public string ActionName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(128)]
        [ProtoMember(3)]
        public string ControllerName { get; set; }
        
        /// <summary>
        /// 小图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(4)]
        public string SmallIcon { get; set; }

        /// <summary>
        /// 大图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(5)]
        public string BigIcon { get; set; }

        /// <summary>
        /// 访问地址
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        public string URL { get; set; }

        /// <summary>
        /// 是否Ext页面
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public bool IsExtPage { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public Guid ApplicationId { get; set; }
       
        [DataMember]
        [ProtoMember(10)]
        public string ParentName { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public TenantType? TenantType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [ProtoMember(12)]
        public string Description { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public TenantVersion Version { get; set; }
        /// <summary>
        /// 菜单参数
        /// </summary>
        [DataMember]
        [ProtoMember(14)]
        public string Parameters { get; set; }

        [DataMember]
        [ProtoMember(15)]
        public string DefaultRoleId { get; set; }

        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        [DataMember]
        [ProtoMember(16)]
        public string AuthorityId { get; set; }

        public string iconCls { get { return SmallIcon; } }

        public bool Equals(MenuNodeSimpleDTO x, MenuNodeSimpleDTO y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(MenuNodeSimpleDTO obj)
        {
            return obj.Id.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
