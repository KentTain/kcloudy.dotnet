using System;
using System.Reflection;
using System.Text;

namespace KC.Service.Util
{
    public static class CacheKeyUtil
    {
        public static Func<string, MethodBase, object[], string> CacheKeyGenerator = CacheKeyGenerator = (tenantName, method, inputs) =>
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}-", !string.IsNullOrWhiteSpace(tenantName) ? tenantName.ToLower() : "com");
            sb.AppendFormat("{0}-", method.DeclaringType != null ? method.DeclaringType.FullName.ToLower() : "namespace");
            sb.AppendFormat("{0}-", method.Name.ToLower());
            if (inputs != null)
            {
                Array.ForEach(inputs, input =>
                {
                    if (input != null)
                    {
                        if (input is string
                            || input is int
                            || input is float
                            || input is decimal)
                        {
                            sb.AppendFormat("{0}-", input.ToString());
                        }
                        else
                        {
                            string hashCode = (input == null) ? "" : input.GetHashCode().ToString();
                            sb.AppendFormat("{0}-", hashCode);
                        }
                    }
                    
                });
            }
            return sb.ToString().TrimEnd('-');
        };
    }
}
