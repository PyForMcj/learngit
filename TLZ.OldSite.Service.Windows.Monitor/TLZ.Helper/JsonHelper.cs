#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称： 
     * 文件名：  JsonHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014/7/15
     * 描述    : Json帮助类
     * =====================================================================
     * 修改时间：2014/7/15
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TLZ.Helper
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss时间格式
        /// </summary>
        public static readonly IsoDateTimeConverter TimeFormat = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
        /// <summary>
        /// 对象转Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertJsonToStr(object obj)
        {
            if (obj == null)
                return string.Empty;
            string result = string.Empty;
            try
            {
                result = JsonConvert.SerializeObject(obj);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return result;
        }
        public static string ConvertJsonToStr(object obj, IsoDateTimeConverter isoDateTimeConverter)
        {
            if (obj == null)
                return string.Empty;
            string result = string.Empty;
            try
            {
                result = JsonConvert.SerializeObject(obj, isoDateTimeConverter);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return result;
        }

        public static string ConvertJsonToStr(object obj, JsonSerializerSettings jsonSerializerSettings)
        {
            if (obj == null)
                return string.Empty;
            string result = string.Empty;
            try
            {
                result = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return result;
        }
        /// <summary>
        /// 对象转Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatting"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string ConvertJsonToStr(object obj, params JsonConverter[] converters)
        {
            if (obj == null)
                return string.Empty;
            string result = string.Empty;
            try
            {

                result = JsonConvert.SerializeObject(obj, converters);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return result;
        }

        /// <summary>
        /// Json字符串转对象，该方法仅在反射时调用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ConvertStrToJsonT<T>(string str)
        {
            return ConvertStrToJson<T>(str);
        }
        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ConvertStrToJson<T>(string str)
        {
            return ConvertStrToJson<T>(str, new JsonConverter[0]);
        }
        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static T ConvertStrToJson<T>(string str, params JsonConverter[] converters)
        {
            if (string.IsNullOrWhiteSpace(str))
                return default(T);
            T t = default(T);
            try
            {
                t = JsonConvert.DeserializeObject<T>(str, converters);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return t;
        }
        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JObject ConvertStrToJson(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            JObject jobj = null;
            try
            {
                jobj = JObject.Parse(str);
            }
            catch (JsonReaderException ex1)
            {
                Debug.WriteLine(ex1);
                throw ex1;
            }
            catch (JsonWriterException ex2)
            {
                Debug.WriteLine(ex2);
                throw ex2;
            }
            catch (Exception ex3)
            {
                Debug.WriteLine(ex3);
                throw ex3;
            }
            return jobj;
        }
        /// <summary>
        /// 将Json字符串转换为指定类型的对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertStrToObject(string str)
        {
            return JsonConvert.DeserializeObject(str);
        }
        /// <summary>
        /// 将Json字符串转换为指定类型的对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertStrToJson(string str, string type)
        {
            return ConvertStrToJson(str, Type.GetType(type));
        }
        /// <summary>
        /// 将Json字符串转换为指定类型的对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertStrToJson(string str, Type type)
        {
            return ConvertStrToJson(str, type, new JsonConverter[0]);
        }
        /// <summary>
        /// 将Json字符串转换为指定类型的对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="jsonConverts"></param>
        /// <returns></returns>
        public static object ConvertStrToJson(string str, Type type, params JsonConverter[] jsonConverts)
        {
            return JsonConvert.DeserializeObject(str, type, jsonConverts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T ConvertStrToT<T>(string str, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType(str, anonymousTypeObject);
        }
        /// <summary>
        /// 将对象序列化到文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        public static void SerializerToFile(string filePath, object obj)
        {
            if (string.IsNullOrEmpty(filePath) || obj == null)
                return;
            string str = ConvertJsonToStr(obj);
            if (string.IsNullOrEmpty(str))
                return;
            File.WriteAllText(filePath, str, Encoding.UTF8);
        }
        /// <summary>
        /// 将文件反序列化到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string filePath)
        {
            T result = default(T);
            if (string.IsNullOrEmpty(filePath))
                return result;
            if (!File.Exists(filePath))
                return result;
            string str = File.ReadAllText(filePath);
            return ConvertStrToJson<T>(str);
        }
        /// <summary>
        /// 通过JSON（属性/值）格式的字符串设置对象的属性值
        /// </summary>
        /// <param name="srcObject">要设置的原对象</param>
        /// <param name="newPropertyValueJson">JSON（属性/值）字符串</param>
        public static void ObjectSetPropertyValue(object srcObject, string newPropertyValueJson)
        {
            if (srcObject == null || string.IsNullOrWhiteSpace(newPropertyValueJson))
                return;
            JObject jObject = JsonHelper.ConvertStrToJson<JObject>(newPropertyValueJson);
            if (jObject == null)
                return;
            PropertyInfo[] propertyInfos = srcObject.GetType().GetProperties();
            IEnumerable<JProperty> properties = jObject.Properties();
            if (propertyInfos != null && properties != null)
            {
                foreach (JProperty jProperty in properties)
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (string.Compare(propertyInfo.Name, jProperty.Name, true) == 0)
                        {
                            if (jProperty.Value is JValue)
                            {
                                JValue jValue = jProperty.Value as JValue;
                                object objValue = jValue.Value;
                                if (objValue != null)
                                {
                                    Type propertyType = propertyInfo.PropertyType;
                                    TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyType);
                                    try
                                    {
                                        object value = typeConverter.ConvertFromString(objValue.ToString());
                                        propertyInfo.SetValue(srcObject, value, null);
                                    }
                                    catch (NotSupportedException)
                                    {
                                    }
                                    catch (ArgumentException)
                                    {
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

    }
}
