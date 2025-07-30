using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using ProtoBuf.Meta;

namespace KC.Service.Util
{
    public static class ProtobufUtil
    {
        public static void InitProtobufSerialize()
        {
            //ProtoBuf配置：对象继承映射
            var protoConfig = RuntimeTypeModel.Default;
            protoConfig.AutoCompile = false;
            //Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354
            //TODO: 需要返回TenantUserDTO
            //protoConfig.Add(typeof(Tenant), true).AddSubType(99, typeof(KC.Model.Admin.TenantUser));
            protoConfig.Add(typeof(TenantApiAccessInfo), true).AddSubType(100, typeof(Tenant));

            //protoConfig.Add(typeof(ConfigBase), true).AddSubType(101, typeof(EmailConfig));
            //protoConfig.Add(typeof(ConfigBase), true).AddSubType(102, typeof(SmsConfig));
            //protoConfig.Add(typeof(ConfigBase), true).AddSubType(103, typeof(CallConfig));
            //protoConfig.Add(typeof(ConfigBase), true).AddSubType(104, typeof(WeixinConfig));

            protoConfig.Add(typeof(TreeNodeDTO<OrganizationDTO>), true).AddSubType(101, typeof(OrganizationDTO));
            protoConfig.Add(typeof(TreeNodeDTO<PermissionDTO>), true).AddSubType(102, typeof(PermissionDTO));
            protoConfig.Add(typeof(TreeNodeDTO<MenuNodeDTO>), true).AddSubType(103, typeof(MenuNodeDTO));
            //protoConfig.Add(typeof(TreeNodeDTO<IndustryClassficationDTO>), true).AddSubType(104, typeof(IndustryClassficationDTO));
            //protoConfig.Add(typeof(TreeNodeDTO<CategoryDTO>), true).AddSubType(105, typeof(CategoryDTO));
            //protoConfig.Add(typeof(TreeNodeDTO<ArticleCategoryDTO>), true).AddSubType(106, typeof(ArticleCategoryDTO));

            protoConfig.Add(typeof(TreeNodeSimpleDTO<PermissionSimpleDTO>), true).AddSubType(111, typeof(PermissionSimpleDTO));
            protoConfig.Add(typeof(TreeNodeSimpleDTO<MenuNodeSimpleDTO>), true).AddSubType(112, typeof(MenuNodeSimpleDTO));

            //protoConfig.Add(typeof(PropertyAttributeBaseDTO), true).AddSubType(121, typeof(CustomerExtInfoDTO));
            //protoConfig.Add(typeof(PropertyAttributeBaseDTO), true).AddSubType(122, typeof(CustomerExtInfoProviderDTO));
            //protoConfig.Add(typeof(PropertyAttributeBaseDTO), true).AddSubType(123, typeof(ServiceAttributeDTO));
            //protoConfig.Add(typeof(PropertyAttributeBaseDTO), true).AddSubType(124, typeof(ServiceProviderAttributeDTO));
            //protoConfig.Add(typeof(PropertyAttributeBaseDTO), true).AddSubType(125, typeof(CompanyProfileDTO));

            //protoConfig.Add(typeof(PropertyBaseDTO<ServiceAttributeDTO>), true).AddSubType(131, typeof(ServiceDTO));
            //protoConfig.Add(typeof(PropertyBaseDTO<ServiceProviderAttributeDTO>), true).AddSubType(132, typeof(ServiceProviderDTO));

        }
    }
}
