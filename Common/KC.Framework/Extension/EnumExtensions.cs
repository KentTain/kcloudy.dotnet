using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KC.Framework.Extension
{
    /// <summary>
    ///     枚举扩展方法类
    /// </summary>
    public static class EnumExtensions
    {
        private static readonly Dictionary<string, List<string>> _descriptionListCache = new Dictionary<string, List<string>>();
        /// <summary>
        ///     获取枚举项的Description特性的描述文字
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static string ToDescription(this Enum enumeration)
        {
            string description = string.Empty;
            Type type = enumeration.GetType();
            var fields = type.GetCustomAttributesData();

            if (!fields.Where(i => i.Constructor.DeclaringType.Name == "FlagsAttribute").Any())
            {
                MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
                if (members.Length > 0)
                {
                    return members[0].ToDescription();
                }
                return enumeration.CastTo<string>();
            }

            DescriptionAttribute dna = null;
            FieldInfo fi = null;
            GetEnumValuesFromFlagsEnum(enumeration).ToList().ForEach(i =>
            {
                fi = type.GetField(Enum.GetName(type, i));
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    description += dna.Description + ",";
            });

            return description.EndsWith(",")
                ? description.Remove(description.LastIndexOf(','))
                : description;

        }

        /// <summary>
        ///     获取枚举项的DisplayName特性的描述文字
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static string ToDisplayName(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0)
            {
                return members[0].ToDisplayName();
            }
            return enumeration.CastTo<string>();
        }

        public static List<string> GetEnumDescriptions(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            string typename = type.FullName;
            if (!_descriptionListCache.ContainsKey(typename))
            {
                var fields = type.GetFields().Where(field => field.IsLiteral);
                var values = new List<string>();
                foreach (var field in fields)
                {
                    var a = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (a != null && a.Length > 0)
                    {
                        values.Add(a[0].Description);
                    }
                    else
                    {
                        values.Add(field.Name);
                    }
                }

                _descriptionListCache[typename] = values;
            }

            return _descriptionListCache[typename];
        }

        public static List<string> GetEnumValues(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            var itemList = new List<string>();

            var listOfValues = Enum.GetValues(type);
            foreach (var value in listOfValues)
            {
                itemList.Add(value.ToString());
            }

            return itemList;
        }


        public static T GetEnumFromDescription<T>(string description)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("description");
            }

            Type type = typeof(T);
            var fieldInfos = type.GetFields().Where(field => field.IsLiteral);
            foreach (var field in fieldInfos)
            {
                var a = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (a != null && a.Length > 0)
                {
                    if (a[0].Description.ToLower() == description.ToLower())
                    {
                        return (T)Enum.Parse(typeof(T), field.Name, true);
                    }
                }
                else if (field.Name.ToLower() == description.ToLower())
                {
                    return (T)Enum.Parse(typeof(T), field.Name, true);
                }
            }

            return (T)Enum.Parse(typeof(T), fieldInfos.First().Name, true);
        }

        public static IEnumerable<T> GetEnumList<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="exceptEnums">排除的Enum列表</param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumList<T>(List<T> exceptEnums)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var enumList = Enum.GetValues(typeof(T)).Cast<T>();
            return enumList.Except(exceptEnums);
        }

        public static Dictionary<int, string> GetEnumDictionary<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            Type type = typeof (T);

            var list = new Dictionary<int, string>();
            var enumList = Enum.GetValues(type).Cast<T>();
            foreach (var aEnum in enumList)
            {
                MemberInfo[] members = type.GetMember(aEnum.CastTo<string>());
                if (members.Length > 0)
                {
                    list.Add(aEnum.CastTo<int>(), members[0].ToDescription());
                }
                else
                {
                    list.Add(aEnum.CastTo<int>(), aEnum.CastTo<string>());
                }
            }

            return list;
        }

        public static Dictionary<int, string> GetEnumDictionary<T>(List<T> exceptEnums)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            Type type = typeof(T);

            var list = new Dictionary<int, string>();
            var enumList = Enum.GetValues(type).Cast<T>();
            foreach (var aEnum in enumList.Except(exceptEnums))
            {
                MemberInfo[] members = type.GetMember(aEnum.CastTo<string>());
                if (members.Length > 0)
                {
                    list.Add(aEnum.CastTo<int>(), members[0].ToDescription());
                }
                else
                {
                    list.Add(aEnum.CastTo<int>(), aEnum.CastTo<string>());
                }
            }

            return list;
        }

        /// <summary>
        /// 获取指定的自定义属性
        /// </summary>
        /// <typeparam name="T">自定义属性类型</typeparam>
        /// <typeparam name="S">Enum类型</typeparam>
        /// <param name="enumValue">enum的字符串值</param>
        /// <returns></returns>
        public static T GetCustomAttr<T, S>(string enumValue) where T:Attribute
        {
            if (string.IsNullOrEmpty(enumValue))
            {
                throw new ArgumentNullException("enumValue");
            }
            Type type = typeof(S);
            var attrObj = type.GetField(enumValue).GetAttribute<T>(false);
            return attrObj;
        }

        public static T GetCustomAttr<T>(this Enum enumeration) where T:Attribute
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0)
            {
                var desc = members[0].GetAttribute<T>(false);
                return desc;
            }
            return null;
        }

        /// <summary>
        /// 得到Flags特性的枚举的集合
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<Enum> GetEnumValuesFromFlagsEnum(this Enum enumeration)
        {
            List<Enum> values = Enum.GetValues(enumeration.GetType()).Cast<Enum>().ToList();
            List<Enum> res = new List<Enum>();
            foreach (var itemValue in values)
            {
                if ((enumeration.GetHashCode() & itemValue.GetHashCode()) != 0)
                    res.Add(itemValue);
            }
            return res;
        }
    }
}
