using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using KC.Framework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

namespace KC.Common
{
    /// <summary>
    /// 将UTC的日期格式转换为Beijing的东八区日期格式（Newtonsoft.Json）
    /// </summary>
    public class DateTimeNewtonJsonConverter : Newtonsoft.Json.JsonConverter<DateTime>
    {
        public override DateTime ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                if (reader.Value is DateTime dtValue)
                {
                    if (dtValue == DateTime.MinValue)
                        return dtValue;

                    return dtValue.AddHours(-8);
                }
                else if (reader.Value is long it)
                {
                    var dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1));
                    var ltime = it * 10000;
                    var tsNow = new TimeSpan(ltime);
                    return dtStart.Add(tsNow);
                }
                else
                {
                    var s = reader.Value.ToString();
                    if (DateTime.TryParse(s, out DateTime outDt))
                    {
                        return outDt.AddHours(-8);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return DateTime.MinValue;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, DateTime value, Newtonsoft.Json.JsonSerializer serializer)
        {
            string s = value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteValue(s);
        }
    }
    /// <summary>
    /// 将布尔格式（true/false、True/False、1/0）转换为布尔型（Newtonsoft.Json）
    /// </summary>
    public class BooleanNewtonJsonConverter : Newtonsoft.Json.JsonConverter<bool>
    {
        public override bool ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                if (reader.Value is bool dtValue)
                {
                    return dtValue;
                }
                else if (reader.Value is int it)
                {
                    return it == 1;
                }
                else
                {
                    var s = reader.Value.ToString();
                    if (bool.TryParse(s, out bool outDt))
                    {
                        return outDt;
                    }

                    if (s.Equals("true") || s.Equals("True") || s.Equals("是") || s.Equals("1"))
                        return true;
                    if (s.Equals("false") || s.Equals("Frue") || s.Equals("否") || s.Equals("0"))
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return false;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, bool value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
    /// <summary>
    /// 将UTC的日期格式转换为Beijing的东八区日期格式（System.Text.Json）
    /// </summary>
    public class DateTimeJsonConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        public override DateTime Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            try
            {
                if (reader.TryGetDateTime(out DateTime dtValue))
                {
                    if (dtValue == DateTime.MinValue)
                        return dtValue;

                    return dtValue.AddHours(-8);
                }
                else if (reader.TryGetInt64(out long it))
                {
                    var dtStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1));
                    var ltime = it * 10000;
                    var tsNow = new TimeSpan(ltime);
                    return dtStart.Add(tsNow);
                }

                string s = reader.GetString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (DateTime.TryParse(s, out DateTime outDt))
                    {
                        return outDt.AddHours(-8);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return DateTime.MinValue;
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, DateTime value, System.Text.Json.JsonSerializerOptions options)
        {
            string s = value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteStringValue(s);
        }
    }
    /// <summary>
    /// 将UTC的日期格式转换为Beijing的东八区日期格式（System.Text.Json）
    /// </summary>
    public class BoolJsonConverter : System.Text.Json.Serialization.JsonConverter<bool>
    {
        public override bool Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            try
            {
                if (reader.TryGetInt32(out int it))
                {
                    return it == 1;
                }

                var s = reader.GetString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (bool.TryParse(s, out bool outDt))
                    {
                        return outDt;
                    }

                    if (s.Equals("true") || s.Equals("True") || s.Equals("是") || s.Equals("1"))
                        return true;
                    if (s.Equals("false") || s.Equals("Frue") || s.Equals("否") || s.Equals("0"))
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return false;
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, bool value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
    /// <summary>
    /// 提供了一个关于对象序列化的辅助类
    /// </summary>
    public static class SerializeHelper
    {
        #region Newtonsoft.Json序列化
        /// <summary>
        /// Json序列化设置对象
        /// </summary>
        /// <param name="isToLocalDateTime">是否转换为北京时间</param>
        /// <param name="isStringEnum">是否将Enum转换为String</param>
        /// <param name="isMissingMember">是否忽略缺失成员</param>
        /// <returns></returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(bool isToLocalDateTime = true, bool isStringEnum = true, bool isMissingMember = true)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            if (isMissingMember)
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            if (isStringEnum)
                settings.Converters.Add(new StringEnumConverter());
            if (isToLocalDateTime)
                settings.Converters.Add(new Common.DateTimeNewtonJsonConverter());

            settings.Converters.Add(new Common.BooleanNewtonJsonConverter());
            return settings;
        }

        /// <summary>
        /// 类对像转换成json格式
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isToLocalDateTime">是否转换为北京时间</param>
        /// <param name="isStringEnum">是否将Enum转换为String</param>
        /// <param name="isMissingMember">是否忽略缺失成员</param>
        /// <returns></returns>
        public static string ToJson<T>(T t, bool isToLocalDateTime = true, bool isStringEnum = false, bool isMissingMember = true)
        {
            var jsonSetting = GetJsonSerializerSettings(isToLocalDateTime, isStringEnum, isMissingMember);
            return JsonConvert.SerializeObject(t, jsonSetting);
        }

        /// <summary>
        /// json格式转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <param name="isToLocalDateTime">是否转换为北京时间</param>
        /// <param name="isStringEnum">是否将Enum转换为String</param>
        /// <param name="isMissingMember">是否忽略缺失成员</param>
        /// <returns></returns>
        public static T FromJson<T>(string strJson, bool isToLocalDateTime = true, bool isStringEnum = false, bool isMissingMember = true)
        {
            if (string.IsNullOrEmpty(strJson)) return default;

            var jsonSetting = GetJsonSerializerSettings(isToLocalDateTime, isStringEnum, isMissingMember);
            return JsonConvert.DeserializeObject<T>(strJson, jsonSetting);
        }

        /// <summary>
        /// json格式转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <param name="isToLocalDateTime">是否转换为北京时间</param>
        /// <param name="isStringEnum">是否将Enum转换为String</param>
        /// <param name="isMissingMember">是否忽略缺失成员</param>
        /// <returns></returns>
        public static object FromJson(Type type, string strJson, bool isToLocalDateTime = true, bool isStringEnum = false, bool isMissingMember = true)
        {
            if (string.IsNullOrEmpty(strJson)) return null;

            var jsonSetting = GetJsonSerializerSettings(isToLocalDateTime, isStringEnum, isMissingMember);
            return JsonConvert.DeserializeObject(strJson, type, jsonSetting);
        }

        /// <summary>
        /// 将数组对象的Json字符串转换为字典列表<>
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetDictListByArrayJson(string strJson)
        {
            if (string.IsNullOrEmpty(strJson)) return null;

            var result = new List<Dictionary<string, string>>();
            var objStringList = JsonConvert.DeserializeObject<List<object>>(strJson);
            foreach (var objString in objStringList)
            {
                var dicObj = new Dictionary<string, string>();
                JObject obj = JObject.Parse(objString.ToString());
                IEnumerable<JProperty> properties = obj.Properties();
                foreach (JProperty property in properties)
                {
                    var pName = property.Name;
                    JToken value = obj[pName];
                    dicObj.Add(pName, value.ToString());
                }
                result.Add(dicObj);
            }

            return result;
        }

        /// <summary>
        /// 将List对象的Json字符串转换为Map列表<>
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static Hashtable GetMapByJson(string strJson)
        {
            if (string.IsNullOrEmpty(strJson)) return null;
            
            var jsonSetting = GetJsonSerializerSettings();
            JObject obj = (JObject)JsonConvert.DeserializeObject(strJson, jsonSetting);
            if (null == obj)
                return new Hashtable();

            var map = new Hashtable(obj.Count);
            foreach(var keyValuePair in obj)
            {
                map.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return map;
        }

        /// <summary>
        /// 从json字符串中获取某属性的值
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMapValueByKey(string strJson, string key, bool isToLocalDateTime = true, bool isStringEnum = false, bool isMissingMember = true)
        {
            if (string.IsNullOrEmpty(strJson)) return string.Empty;

            var jsonSetting = GetJsonSerializerSettings(isToLocalDateTime, isStringEnum, isMissingMember);
            JObject obj = (JObject)JsonConvert.DeserializeObject(strJson, jsonSetting);
            if (null == obj)
                return string.Empty;

            JToken token = obj[key];
            if (token.Type == JTokenType.Boolean)
                return token.ToString().ToLower();

            return token.ToString();
        }

        /// <summary>
        /// 从json字符串中获取某属性的对象
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetMapObjectByKey<T>(string strJson, string key, bool isToLocalDateTime = true, bool isStringEnum = false, bool isMissingMember = true)
        {
            if (string.IsNullOrEmpty(strJson)) return default;

            var json = GetMapValueByKey(strJson, key, isToLocalDateTime, isStringEnum, isMissingMember);
            if (string.IsNullOrEmpty(json)) return default;

            return FromJson<T>(json, isToLocalDateTime, isStringEnum, isMissingMember);
        }
        #endregion

        #region Protobuf.Net序列化
        /// <summary>
        /// Protobuf.Net序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToProtobufBinary<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Protobuf.Net反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T FromProtobufBinary<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            using (var ms = new MemoryStream(stream))
            {
                T t = Serializer.Deserialize<T>(ms);
                return t;
            }
        }

        public static object FromProtobufBinary(Type type, byte[] stream)
        {
            if (stream == null)
            {
                return null;
            }

            using (var ms = new MemoryStream(stream))
            {
                return Serializer.NonGeneric.Deserialize(type, ms);
            }
        }
        #endregion

        #region BinaryFormatter序列化

        public static byte[] ToBinary<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        public static T FromBinary<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
        #endregion
    }
}
