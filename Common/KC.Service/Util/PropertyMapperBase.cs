using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.Base;

namespace KC.Service.Util
{
    public abstract class PropertyMapperBase<T1, T2> : MapperBase<T1, T2>
        where T2 : class
        where T1 : class
    {
        protected string CheckPropertyAttributeNotEmpty(IEnumerable<PropertyAttributeBase> queryProperties,
            IEnumerable<string> validKeyList)
        {
            var sbMessage = new StringBuilder();
            foreach (var keyName in validKeyList)
            {
                //验证查询参数是否有该Key
                bool isValid = queryProperties.Any(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (!isValid)
                {
                    sbMessage.AppendLine("查询条件缺少必须提供的参数Key，其Key值为：" + keyName + "。");
                    continue;
                }

                //验证查询参数该Key是否为空
                foreach (
                    var property in
                        queryProperties.Where(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        sbMessage.AppendLine("查询条件必须参数Key（" + keyName + "）的值为空，请提供该键值。");
                    }
                }
            }

            return sbMessage.ToString();
        }

        protected string CheckPropertyAttributeIsDateTime(IEnumerable<PropertyAttributeBase> queryProperties,
            IEnumerable<string> validKeyList)
        {
            var sbMessage = new StringBuilder();
            foreach (var keyName in validKeyList)
            {
                //验证查询参数是否有该Key
                bool isValid = queryProperties.Any(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (isValid)
                {
                    //验证查询参数该Key是否为空
                    foreach (
                        var property in
                            queryProperties.Where(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!string.IsNullOrEmpty(property.Value) && !property.Value.IsDate())
                        {
                            sbMessage.AppendLine("查询条件参数Key（" + keyName + "）的值不为日期型数据，请提供正确的数据。");
                        }
                    }
                }
            }

            return sbMessage.ToString();
        }

        protected string CheckPropertyAttributeIsInt(IEnumerable<PropertyAttributeBase> queryProperties,
            IEnumerable<string> validKeyList)
        {
            var sbMessage = new StringBuilder();
            foreach (var keyName in validKeyList)
            {
                //验证查询参数是否有该Key
                bool isValid = queryProperties.Any(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (isValid)
                {
                    //验证查询参数该Key是否为空
                    foreach (
                        var property in
                            queryProperties.Where(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!string.IsNullOrEmpty(property.Value) && !property.Value.IsInt())
                        {
                            sbMessage.AppendLine("查询条件参数Key（" + keyName + "）的值不为整型数据，请提供正确的数据。");
                        }
                    }
                }
            }

            return sbMessage.ToString();
        }

        protected string CheckPropertyAttributeIsDecimal(IEnumerable<PropertyAttributeBase> queryProperties,
            IEnumerable<string> validKeyList)
        {
            var sbMessage = new StringBuilder();
            foreach (var keyName in validKeyList)
            {
                //验证查询参数是否有该Key
                bool isValid = queryProperties.Any(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (isValid)
                {
                    //验证查询参数该Key是否为空
                    foreach (
                        var property in
                            queryProperties.Where(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!string.IsNullOrEmpty(property.Value) && !property.Value.IsDecimal())
                        {
                            sbMessage.AppendLine("查询条件参数Key（" + keyName + "）的值不为数值型数据，请提供正确的数据。");
                        }
                    }
                }
            }

            return sbMessage.ToString();
        }

        protected string CheckPropertyAttributeIsBoolean(IEnumerable<PropertyAttributeBase> queryProperties,
            IEnumerable<string> validKeyList)
        {
            var sbMessage = new StringBuilder();
            foreach (var keyName in validKeyList)
            {
                //验证查询参数是否有该Key
                bool isValid = queryProperties.Any(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                if (isValid)
                {
                    //验证查询参数该Key是否为空
                    foreach (
                        var property in
                            queryProperties.Where(q => q.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!string.IsNullOrEmpty(property.Value) && !property.Value.IsBoolean())
                        {
                            sbMessage.AppendLine("查询条件参数Key（" + keyName + "）的值不为布尔型数据，请提供正确的数据。");
                        }
                    }
                }
            }

            return sbMessage.ToString();
        }

        protected List<PropertyAttributeBase> GetPropertyAttributesByPropertyName<T>(IEnumerable<T> propeties,
            IEnumerable<string> validKeyList) where T : PropertyBase<PropertyAttributeBase>
        {
            var canParser = typeof(T).IsSubclassOfRawClass(typeof(PropertyBase<PropertyAttributeBase>));
            if (!canParser)
                throw new ComponentException("T is not derive from the type " + typeof(PropertyBase<PropertyAttributeBase>).FullName);

            return
                propeties.Where(attribute => validKeyList.Contains(attribute.Name))
                    .SelectMany(query => query.PropertyAttributeList).ToList();
        }

        protected string GetPropertyAttributeValueByKey<T>(IEnumerable<T> propeties, string filterKey) where T : PropertyBase<PropertyAttributeBase>
        {
            var canParser = typeof(T).IsSubclassOfRawClass(typeof(PropertyBase<PropertyAttributeBase>));
            if (!canParser)
                throw new ComponentException("T is not derive from the type " + typeof(PropertyBase<PropertyAttributeBase>).FullName);

            return propeties.Select(query =>
            {
                var firstOrDefault =
                    query.PropertyAttributeList.FirstOrDefault(
                        a => a.Name == filterKey);
                return firstOrDefault != null ? firstOrDefault.Value : string.Empty;
            }).FirstOrDefault();
        }

        protected List<string> GetPropertyAttributeValueListByKey<T>(IEnumerable<T> propeties, string filterKey)
            where T : PropertyBase<PropertyAttributeBase>
        {
            var canParser = typeof(T).IsSubclassOfRawClass(typeof(PropertyBase<PropertyAttributeBase>));
            if (!canParser)
                throw new ComponentException("T is not derive from the type " + typeof(PropertyBase<PropertyAttributeBase>).FullName);

            return propeties.Select(query =>
            {
                var firstOrDefault =
                    query.PropertyAttributeList.FirstOrDefault(
                        a => a.Name == filterKey);
                return firstOrDefault != null ? firstOrDefault.Value : string.Empty;
            }).Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
    }
}
