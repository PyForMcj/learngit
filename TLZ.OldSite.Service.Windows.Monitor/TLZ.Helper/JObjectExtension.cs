using System;
using System.Linq;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
    public static class JObjectExtension
    {
        /// <summary>
        /// 获得指定Key的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="jObject">JSON对象</param>
        /// <param name="key">键名称</param>
        /// <returns>JSON对象的值</returns>
        public static T Get<T>(this JObject jObject, string key)
        {
            T result = default(T);
            if (jObject == null)
                return result;
            try
            {
                if (jObject.ContainsKey(key))
                {
                    JToken jToken = jObject[key];
                    if (jToken != null)
                    {
                        result = jToken.Value<T>();
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
            return result;
        }

        /// <summary>
        /// JSON对象是否存在指定键
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        /// <param name="key">键名称</param>
        /// <returns>是否存在</returns>
        public static bool ContainsKey(this JObject jObject, string key)
        {
            bool result = false;
            if (jObject == null)
                return result;
            try
            {
                if (jObject[key] != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
            return result;
        }
        /// <summary>
        /// 往JSON对象中添加或更新一个属性
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="jProperty"></param>
        public static void AddOrUpdate(this JObject jObject, JProperty jProperty)
        {
            if (jObject == null || jProperty == null)
                return;

            try
            {
                if (jObject[jProperty.Name] == null)
                {
                    jObject.Add(jProperty);
                }
                else
                {
                    jObject[jProperty.Name] = jProperty.Value;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
        }
        private static IEnumerable<char> GetCharsInRange(string text, int min, int max)
        {
            return text.Where(e => e >= min && e <= max);
        }

        //var romaji = GetCharsInRange(searchKeyword, 0x0020, 0x007E);
        //var hiragana = GetCharsInRange(searchKeyword, 0x3040, 0x309F);
        //var katakana = GetCharsInRange(searchKeyword, 0x30A0, 0x30FF);
        //var kanji = GetCharsInRange(searchKeyword, 0x4E00, 0x9FBF);
    }
}
