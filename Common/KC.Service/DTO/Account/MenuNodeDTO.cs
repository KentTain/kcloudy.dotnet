using System;
using System.Collections;
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
    public class MenuNodeDTO : TreeNodeDTO<MenuNodeDTO>, IEqualityComparer<MenuNodeDTO>, ICloneable
    {
        public MenuNodeDTO()
        {
            Role = new List<RoleDTO>();
        }

        [DataMember]
        [ProtoMember(99)]
        public bool IsEditMode { get; set; }
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
        /// 菜单参数
        /// </summary>
        [DataMember]
        [MaxLength(1024)]
        [ProtoMember(4)]
        public string Parameters { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(5)]
        public string SmallIcon { get; set; }

        /// <summary>
        /// 大图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(6)]
        public string BigIcon { get; set; }

        /// <summary>
        /// 访问地址
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public string URL { get; set; }

        /// <summary>
        /// 是否Ext页面
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        public bool IsExtPage { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [ProtoMember(9)]
        public string Description { get; set; }
        
        [DataMember]
        [ProtoMember(11)]
        public Guid ApplicationId { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public string ApplicationName { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public string ParentName { get; set; }
        [DataMember]
        [ProtoMember(14)]
        public TenantType? TenantType { get; set; }
        [DataMember]
        [ProtoMember(15)]
        public TenantVersion Version { get; set; }

        [DataMember]
        [ProtoMember(16)]
        public MenuNodeDTO Parent { get; set; }
        //[DataMember]
        //[ProtoMember(14)]
        public List<RoleDTO> Role { get; set; }
        [DataMember]
        [ProtoMember(18)]
        public string DefaultRoleId { get; set; }
        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        [DataMember]
        [ProtoMember(19)]
        public string AuthorityId { get; set; }

        public string iconCls { get { return SmallIcon; }  }

        /// <summary>
        /// 根据ControllerName和ActionName生成URL
        /// </summary>
        /// <returns></returns>
        public static string GetURL(string controllerName, string actionName, string areaName = null)
        {
            return string.Format("/{0}/{1}/{2}", areaName, controllerName, actionName);
        }

        public bool Equals(MenuNodeDTO x, MenuNodeDTO y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(MenuNodeDTO obj)
        {
            return obj.Id.GetHashCode();
        }

        public override object Clone()
        {
            return this.MemberwiseClone();  
        }
    }
}
