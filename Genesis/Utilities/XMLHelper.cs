using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genesis.Logging;

namespace Genesis.Utilities
{
    public class XMLHelper
    {
        public static ILogger Logger = new GenesisLogger();

        /// <summary>  
        /// 序列化XML文件  
        /// </summary>  
        /// <param name="type">类型</param>  
        /// <param name="obj">对象</param>  
        /// <returns></returns>  
        public static string SerializerToXML<T>(object obj)
        {
            try
            {
                using (MemoryStream Stream = new MemoryStream())
                {
                    //创建序列化对象  
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    //序列化对象  
                    xml.Serialize(Stream, obj);
                    Stream.Position = 0;
                    using (StreamReader sr = new StreamReader(Stream))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return string.Empty;
        }

        /// <summary>  
        /// 反序列化  
        /// </summary>  
        /// <param name="type">类型</param>  
        /// <param name="xml">XML字符串</param>  
        /// <returns></returns>  
        public static T DeSerializeToObject<T>(string xml)
        {
            T obj = default(T);
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return obj;
        }

        /// <summary>
        /// 将XML格式文件转换为DataSet
        /// </summary>
        /// <param name="strXML">XML格式</param>
        /// <returns></returns>
        /// <remarks>
        /// Create by Scenery 2012-11-21
        /// </remarks>
        public static DataSet XmlToDataSet(string strXML)
        {
            try
            {
                if (strXML != null)
                {
                    using (StringReader sr = new StringReader(strXML.ToString()))
                    {
                        DataSet ds_Info = new DataSet();
                        ds_Info.ReadXml(sr, XmlReadMode.InferSchema);
                        if (ds_Info.Tables.Count > 0)
                        {
                            return ds_Info;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// 通过同步Url将获取的Xml字符串转换为DOM
        /// </summary>
        /// <param name="sync_url">同步Url</param>
        /// <remarks>Create By Scenery 2012-05-08</remarks>
        public static XmlDocument XmlToDocument(string sync_url, int timeOut = 0)
        {
            //if (!string.IsNullOrEmpty(sync_url))
            //{
            //    try
            //    {
            //        string sbStr = HttpHelper.TransferSync_Get(sync_url, timeOut);
            //        if (sbStr != null)
            //        {
            //            XmlDocument xml_Doc = new XmlDocument();
            //            xml_Doc.LoadXml(sbStr);
            //            return xml_Doc;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Error(ex);
            //    }
            //}

            return null;
        }

        /// <summary>
        /// 通过同步Url将获取的Xml字符串转换为DOM
        /// </summary>
        /// <param name="sync_url">同步Url</param>
        /// <remarks>Create By Scenery 2012-05-08</remarks>
        public static XmlDocument XmlToDocument(string sync_url, string sync_par)
        {
            if (!string.IsNullOrEmpty(sync_url))
            {
                try
                {
                    string strXML = HttpHelper.TransferSync_Post(sync_url,"" ,sync_par);
                    if (strXML != null)
                    {
                        XmlDocument xml_Doc = new XmlDocument();
                        xml_Doc.LoadXml(strXML);
                        return xml_Doc;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            return null;
        }
    }
}