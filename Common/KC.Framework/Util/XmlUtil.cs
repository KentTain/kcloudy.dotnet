using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KC.Framework.Util
{
    public interface ISerialize
    {
        /// <summary>
        /// xml 转换为 model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        T XmlToModel<T>(string xml);


        /// <summary>
        /// model 转换为xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        string ModelToXml<T>(T model);


        /// <summary>
        /// xml 转换为Table
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        DataTable XmlToTable(string xml);


        /// <summary>
        /// 获取对应XML节点的值
        /// </summary>
        /// <param name="stringRoot"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        string XmlAnalysis(string stringRoot, string xml);
    }

    public class XmlSerialize : ISerialize
    {

        public T XmlToModel<T>(string xml)
        {
            var xmlReader = new StringReader(xml);
            var xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(xmlReader);
        }

        public string ModelToXml<T>(T model)
        {
            var stream = new MemoryStream();
            var xmlSer = new XmlSerializer(typeof(T));
            xmlSer.Serialize(stream, model);

            stream.Position = 0;
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }


        public System.Data.DataTable XmlToTable(string xml)
        {
            var xmlReader = new StringReader(xml);
            var ds = new DataSet();
            ds.ReadXml(xmlReader);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取对应XML节点的值   
        /// </summary>
        /// <param name="stringRoot">XML节点的标记</param>
        /// <param name="xml"></param>
        /// <returns>返回获取对应XML节点的值</returns>
        public string XmlAnalysis(string stringRoot, string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            if (null != doc.DocumentElement)
            {
                var node = doc.DocumentElement.SelectSingleNode(stringRoot);
                if (null != node)
                    return node.InnerXml.Trim();
            }
            return string.Empty;
        }
    }
}
