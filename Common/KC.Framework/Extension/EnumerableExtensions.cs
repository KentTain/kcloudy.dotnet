using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using KC.Framework.LINQHelper;

namespace KC.Framework.Extension
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 比较两个List string是否相等
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool ListEquals(this List<string> list1, List<string> list2)
        {
            if (null == list1 && null == list2)
                return true;
            if (null == list1 || null == list2)
                return false;
            if (list1.Count != list2.Count || !list1.All(list2.Contains))
                return false;
            list1.Sort();
            list2.Sort();
            int nCount = list1.Count;
            for (int n = 0; n < nCount; n++)
            {
                if (0 != string.Compare(list1[n], list2[n], false))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 根据条件对列表进行对象去除重复操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }

        /// <summary>
        /// 根据条件对列表进行对象去除重复操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector,
            IEqualityComparer<V> comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector, comparer));
        }

        /// <summary>
        /// 根据排序字段名称，获取排序后的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public static IEnumerable<T> SingleOrderBy<T>(this IEnumerable<T> source, string sortProperty,
            bool ascending = true)
        {
            #region Old Mehtod

            //Type type = typeof(T);

            //PropertyInfo property = type.GetProperty(sortProperty);
            //if (property == null)
            //    throw new ArgumentException("propertyName", "Not Exist");

            //ParameterExpression param = Expression.Parameter(type, "p");
            //Expression propertyAccessExpression = Expression.MakeMemberAccess(param, property);
            //LambdaExpression orderByExpression = Expression.Lambda(propertyAccessExpression, param);

            //Expression sourceExpression = source.AsQueryable().Expression;
            //string methodName = ascending ? "OrderBy" : "OrderByDescending";

            //MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType }, sourceExpression, Expression.Quote(orderByExpression));

            //return source.AsQueryable().Provider.CreateQuery<T>(resultExp);

            #endregion

            return DynamicQueryable.SingleOrderBy(source.AsQueryable(), sortProperty, @ascending);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string orderings, params object[] values)
        {
            return DynamicQueryable.OrderBy(source.AsQueryable(), orderings, values);
        }

        public static IEnumerable<string> SingleOrderBy(this IEnumerable<string> source, bool ascending = true)
        {
            return ascending 
                ? source.AsQueryable().OrderBy(m => m) 
                : source.AsQueryable().OrderByDescending(m => m);
        }

        /// <summary>
        /// 将List<T>转换为DataTable对象（其中，表头为属性的特性值（Display(Name="列名")/Description("列名")/PropertyName)
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="items">列表对象</param>
        /// <param name="propertyNames">需要包含的属性，值为空时，包含所有的属性</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items, IEnumerable<string> propertyNames = null)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                var name = GetPropertyName(prop);
                var t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    if (propertyNames != null && propertyNames.Any())
                    {
                        var pi = props[i];
                        if (propertyNames.Contains(pi.Name))
                        {
                            values[i] = props[i].GetValue(item, null);
                        }
                    }
                    else
                    {
                        values[i] = props[i].GetValue(item, null);
                    }
                }
                tb.Rows.Add(values);
            }

            return tb;
        }
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        private static string GetPropertyName(PropertyInfo prop)
        {
            var displayName = prop.GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>().FirstOrDefault();
            if (displayName != null) return displayName.Name;

            var desciptionName = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>().FirstOrDefault();
            if (desciptionName != null) return desciptionName.Description;

            return prop.Name;
        }
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        /// <summary>
        /// 将DataTable对象转换为List<T>（其中：DataTable的ColumnName为对象T的属性名）
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="dataTable">DataTable对象</param>
        /// <returns></returns>
        public static IEnumerable<T> FromDataTable<T>(DataTable dataTable) where T : class, new()
        {
            // 定义集合 
            List<T> ts = new List<T>();
            //定义一个临时变量 
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行 
            foreach (DataRow dr in dataTable.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性 
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性 
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量 
                                       //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dataTable.Columns.Contains(tempName))
                    {
                        //取值 
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性 
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                //对象添加到泛型集合中 
                ts.Add(t);
            }
            return ts;
        }

        /// <summary>
        /// 选取某个对象的某个字段，将列表该对象的这个字段转换成：xxxx,yyy,zzz
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string ToCommaSeparatedStringByFilter<T>(this IEnumerable<T> enumeration, Func<T, string> selector)
        {
            var enumerable = enumeration as T[] ?? enumeration.ToArray();
            if (!enumerable.Any())
                return String.Empty;

            IList<T> list = enumerable.ToList();
            return
                list.Select(selector)
                    .Aggregate(new StringBuilder(), (sb, s) => sb.Append(", " + s))
                    .Remove(0, 2)
                    .ToString();
        }

        /// <summary>
        /// 将一个字符串列表，转换成：xxxx, yyy, zzz
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> enumeration)
        {
            Func<string, string> selector = s => s;
            return enumeration.ToCommaSeparatedStringByFilter(selector);
        }

        /// <summary>
        /// 将一个字符串列表，转换成：N'xxxx', N'yyy', N'zzz'
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToCommaSeparatedWhereString(this IEnumerable<string> enumeration)
        {
            if (enumeration.Any())
            {
                var fixList = enumeration.FixStringList("N'", "'");
                Func<string, string> selector = s => s;
                return fixList.ToCommaSeparatedStringByFilter(selector);
            }

            return string.Empty;
        }

        /// <summary>
        /// 将一个int列表，转换成：1, 2, 3
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToCommaSeparatedInt(this IEnumerable<int> enumeration)
        {
            Func<int, string> selector = s => s.ToString();
            return enumeration.ToCommaSeparatedStringByFilter(selector);
        }

        /// <summary>
        /// 将列表中的所有字符添加前缀及后缀
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="prefix">前缀</param>
        /// <param name="subfix">后缀</param>
        /// <returns></returns>
        public static IEnumerable<string> FixStringList(this IEnumerable<string> enumeration, string prefix, string subfix)
        {
            if (!enumeration.Any())
                yield break;

            foreach (var m in enumeration)
            {
                yield return prefix + m + subfix;
            }
        }
        /// <summary>
        /// 将以oldValue开头字符中的替换为已newValue为开头
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="oldValue">老值</param>
        /// <param name="newValue">新值</param>
        /// <returns></returns>
        public static IEnumerable<string> ReplaceFirst(this IEnumerable<string> enumeration, string oldValue, string newValue)
        {
            if (!enumeration.Any())
                yield break;

            foreach (var m in enumeration)
            {
                yield return m.ReplaceFirst(oldValue, newValue);
            }
        }
        /// <summary>
        /// 将以oldValue结尾字符中的替换为已newValue为结尾
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="oldValue">老值</param>
        /// <param name="newValue">新值</param>
        /// <returns></returns>
        public static IEnumerable<string> ReplaceLast(this IEnumerable<string> enumeration, string oldValue, string newValue)
        {
            if (!enumeration.Any())
                yield break;

            foreach (var m in enumeration)
            {
                yield return m.ReplaceLast(oldValue, newValue);
            }
        }
        /// <summary>
        /// 获取以/结尾的Url路径，
        ///     例如：www.xxxx.com/
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static IEnumerable<string> EndWithSlash(this IEnumerable<string> enumeration)
        {
            if (!enumeration.Any())
                yield break;

            foreach (var m in enumeration)
            {
                yield return m.EndWithSlash();
            }
        }

        /// <summary>
        /// 将列表中的对象进行平均分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="groupNum"></param>
        /// <returns></returns>
        public static List<IEnumerable<T>> GetAverageGroupList<T>(this IEnumerable<T> enumeration, int groupNum)
        {
            if (!enumeration.Any())
                return new List<IEnumerable<T>>();

            var listGroup = new List<IEnumerable<T>>();
            for (int i = 0; i < enumeration.Count(); i += groupNum)
            {
                listGroup.Add(enumeration.Skip(i).Take(groupNum));
            }
            return listGroup;
        }
    }
}
