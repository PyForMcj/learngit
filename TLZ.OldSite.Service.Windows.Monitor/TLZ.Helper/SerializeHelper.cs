using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TLZ.Helper
{
    /// <summary>
    /// 序列化相关帮助类
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            string result = string.Empty;
            try
            {
                BinaryFormatter binaryFormat = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    binaryFormat.Serialize(ms, obj);
                    byte[] bs = ms.ToArray();
                    result = Convert.ToBase64String(bs);
                    Array.Clear(bs, 0, bs.Length);
                }
                binaryFormat = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToBytes(object obj)
        {
            byte[] bs = null;
            try
            {
                BinaryFormatter binaryFormat = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    binaryFormat.Serialize(ms, obj);
                    bs = ms.ToArray();
                }
                binaryFormat = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bs;
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string str)
        {
            T t = default(T);
            if (str == string.Empty)
                return t;

            byte[] data = Convert.FromBase64String(str);
            t = DeserializeByBytes<T>(data);
            return t;
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">指定对象类型</typeparam>
        /// <param name="data">字节数组</param>
        /// <param name="isClearData">压缩完成后，是否清除待压缩字节数组里面的内容</param>
        /// <returns>指定类型的对象</returns>
        public static T DeserializeByBytes<T>(byte[] data, bool isClearData = true)
        {
            T t = default(T);
            if (data == null)
                return t;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    t = (T)formatter.Deserialize(ms);
                }
                formatter = null;
                if (isClearData)
                    Array.Clear(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return t;
        }
        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="t"></param>
        public static void FileSerialize(string filePath, object obj)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, obj);
                    fs.Flush();
                    formatter = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 反序列化文件到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T FileDeserialize<T>(string filePath)
        {
            T obj = default(T);
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        fs.Seek(0, SeekOrigin.Begin);
                        formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;

                        object o = formatter.Deserialize(fs);
                        if (o is T)
                            obj = (T)o;
                        formatter = null;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return obj;
        }

        /// <summary>
        /// 序列化到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="t"></param>
        public static void XmlSerialize(string filePath, object obj)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    XmlSerializer xs = new XmlSerializer(obj.GetType());
                    xs.Serialize(writer, obj);
                    writer.Flush();
                    xs = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string filePath)
        {
            T t = default(T);
            if (!File.Exists(filePath))
                return t;
            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8, false))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    Object obj = xs.Deserialize(reader);
                    if (obj is T)
                        t = (T)obj;
                    xs = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return t;
        }

        public static T XmlDeserializeByStream<T>(Stream stream)
        {
            T t = default(T);
            if (stream == null || stream.Length == 0)
                return t;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                Object obj = xs.Deserialize(stream);
                if (obj is T)
                    t = (T)obj;
                xs = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return t;
        }
    }
}
