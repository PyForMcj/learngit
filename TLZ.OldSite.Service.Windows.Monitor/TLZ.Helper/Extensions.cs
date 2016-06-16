using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace System
{
    /// <summary>
    /// DateTime扩展
    /// </summary>
    /// <remarks>
    /// [2013-12-14] Develop by SunLiang.
    /// </remarks>
    public static class DateTimeExtensions
    {
        public static string ToString_yyyyMMddHHmmssffff(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        }
        public static string ToString_yyyyMMddHHmmss(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string ToString_yyyyMMddHHmm(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }
        public static string ToString_yyyyMMddHH(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH");
        }
        public static string ToString_yyyyMMdd(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        public static string ToString_yyyyMM(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM");
        }
        public static string ToString_HHmmssffff(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss.ffff");
        }
        public static string ToString_HHmmss(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }
        public static string ToString_HHmm(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }
        /// <summary>
        /// 获取两个日期间隔的总月
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int MonthDifference(this DateTime dateTime, DateTime other)
        {
            return Math.Abs((dateTime.Year - other.Year - 1) * 12 + (12 - other.Month) + dateTime.Month);
        }
    }
    /// <summary>
    /// String类扩展
    /// </summary>
    /// <remarks>
    /// [2013-12-14] Develop by SunLiang.
    /// </remarks>
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        public static string Formater(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        public static string Formater(this string format, object arg0)
        {
            return string.Format(format, arg0);
        }
        public static string Formater(this string format, string arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }
        public static string Formater(this string format, object arg0, object arg1)
        {
            return string.Format(format, arg0, arg1);
        }
        public static string Formater(this string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0, arg1, arg2);
        }
        public static string Formater(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }
        public static bool IsMatch(this string s, string pattern)
        {
            if (s == null) return false;
            else return Regex.IsMatch(s, pattern);
        }
        public static bool IsMatch(this string s, string pattern, RegexOptions options)
        {
            if (s == null) return false;
            else return Regex.IsMatch(s, pattern, options);
        }
        public static string Match(this string s, string pattern)
        {
            if (s == null) return "";
            return Regex.Match(s, pattern).Value;
        }
        public static bool IsInt(this string s)
        {
            int i;
            return int.TryParse(s, out i);
        }
        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }
        public static string ToCamel(this string s)
        {
            if (s.IsNullOrEmpty()) return s;
            return s[0].ToString().ToLower() + s.Substring(1);
        }
        public static string ToPascal(this string s)
        {
            if (s.IsNullOrEmpty()) return s;
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
        /// <summary>
        /// 将字符串表示形式转换为它的等效 32 位有符号整数，如转换失败返回 0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int TryConvertInt32(this string s)
        {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }
        /// <summary>
        /// 将字符串表示形式转换为它的等效Decimal类型，如转换失败返回 0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal TryConvertDecimal(this string s)
        {
            decimal result = 0;
            decimal.TryParse(s, out result);
            return result;
        }
        /// <summary>
        /// 将字符串表示形式转换为它的等效Double类型，如转换失败返回 0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double TryConvertDouble(this string s)
        {
            double result = 0;
            double.TryParse(s, out result);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short TryConvertShort(this string s)
        {
            short result = 0;
            short.TryParse(s, out result);
            return result;
        }
        /// <summary>
        /// 将字符串按逗号分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T[] SplitToArray<T>(this string s) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new T[0];
            string[] array = s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<T> ids = new List<T>();
            Type t = typeof(T);
            foreach (var item in array)
            {
                ids.Add((T)Convert.ChangeType(item, t));
            }
            return ids.ToArray();
        }
        /// <summary>
        /// 将字符串按<see cref="seperator"/>分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="seperator">seperator</param>
        /// <returns></returns>
        public static T[] SplitToArray<T>(this string s, params char[] seperator) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new T[0];
            string[] array = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            Type t = typeof(T);
            return array.Select(item => (T)Convert.ChangeType(item, t)).ToArray();
        }
        /// <summary>
        /// 将字符串按<see cref="seperator"/>分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="seperator">seperator</param>
        /// <returns></returns>
        public static T[] SplitToArray<T>(this string s, char seperator) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new T[0];
            string[] array = s.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            List<T> ids = new List<T>();
            Type t = typeof(T);
            foreach (var item in array)
            {
                ids.Add((T)Convert.ChangeType(item, t));
            }
            return ids.ToArray();
        }
        /// <summary>
        /// 将字符串按逗号分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<T> SplitToList<T>(this string s) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new List<T>();
            string[] array = s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<T> ids = new List<T>();
            Type t = typeof(T);
            foreach (var item in array)
            {
                ids.Add((T)Convert.ChangeType(item, t));
            }
            return ids;
        }
        /// <summary>
        /// 将字符串按<see cref="seperator"/>分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="seperator">seperator</param>
        /// <returns></returns>
        public static List<T> SplitToList<T>(this string s, params char[] seperator) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new List<T>();
            string[] array = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            List<T> ids = new List<T>();
            Type t = typeof(T);
            foreach (var item in array)
            {
                ids.Add((T)Convert.ChangeType(item, t));
            }
            return ids;
        }
        /// <summary>
        /// 将字符串按<see cref="seperator"/>分割成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="seperator">seperator</param>
        /// <returns></returns>
        public static List<T> SplitToList<T>(this string s, char seperator) where T : IConvertible
        {
            if (s.IsNullOrEmpty()) return new List<T>();
            string[] array = s.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            List<T> ids = new List<T>();
            Type t = typeof(T);
            foreach (var item in array)
            {
                ids.Add((T)Convert.ChangeType(item, t));
            }
            return ids;
        }
    }

    /// <summary>
    /// Type 拓展
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 确定当前实例是否是继承或者实现某个Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInheritOfType(this Type self, Type type)
        {
            if (type.IsInterface)
                return self.GetInterface(type.Name) != null;
            if (type.IsEnum)
                return self.GetEnumUnderlyingType() == type;
            return self.IsSubclassOf(type);
        }
    }

    /// <summary>
    /// 扩展HttpResponse输出JSON格式 
    /// Develop by YangHuanWen
    /// 2015-10-16 
    /// </summary>
    public static class ResponseExtensions
    {

        public static void WriteJson(this HttpResponse httpResponse, object data)
        {
            httpResponse.WriteJson(0, data); ;
        }

        /// <summary>
        /// 创建一个成功的json返回实体
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="data">数据</param>
        /// <param name="code">消息</param>
        /// <returns>Json</returns>
        public static void WriteJson(this HttpResponse httpResponse, int code, object data = null)
        {
            httpResponse.WriteJson(code, data, null);
        }

        /// <summary>
        /// 创建一个成功的json返回实体
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="code">数据</param>
        /// <param name="msg">消息</param>
        /// <returns>Json</returns>
        public static void WriteJson(this HttpResponse httpResponse, int code, string msg)
        {
            httpResponse.WriteJson(code, null, msg); ;
        }

        /// <summary>
        /// 创建一个成功的json返回实体
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="code">错误码</param>
        /// <param name="data">数据</param>
        /// <param name="msg">消息</param>
        /// <returns>Json</returns>
        public static void WriteJson(this HttpResponse httpResponse, int code, object data, string msg)
        {
            string jsonResult = string.Empty; ;
            object obj = new
            {
                Code = code,
                Data = data,
                Message = msg
            };
            try
            {
                jsonResult = JsonConvert.SerializeObject(obj);
            }
            catch (JsonReaderException ex1)
            {
                throw ex1;
            }
            httpResponse.Write(jsonResult);
        }
    }

    public static class EnumExtensions
    {
        //[Obsolete("该方法已经过时，请参考使用 GetName() 或者 GetText() 或者 GetValue() 方法.", false)]
        //public static string AsString(this Enum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());
        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    if ((attributes != null) && (attributes.Length > 0))
        //        return attributes[0].Description;
        //    else
        //        return value.ToString();
        //}
        /// <summary>
        /// 获取枚举值的名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }
        /// <summary>
        ///  获取该枚举的显示值（如果使用了DescriptionAttribute 标签则显示描述中的别名，否则使用 Enum 的名称。）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetText(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }
        /// <summary>
        ///  获取该枚举的  Description 标记值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return null;
        }

        /// <summary>
        /// 获取枚举代表的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }
        public static T Parse<T>(this Enum enumThis, int value)
        {
            return (T)Enum.Parse(enumThis.GetType(), value.ToString());
        }
        public static T Parse<T>(this Enum enumThis, string value)
        {
            return (T)Enum.Parse(enumThis.GetType(), value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindComboxEnumType<T>
    {
        public string Name
        {
            get;
            set;
        }

        public string TypeValue
        {
            get;
            set;
        }

        private static readonly List<BindComboxEnumType<T>> bindTypes;

        static BindComboxEnumType()
        {
            bindTypes = new List<BindComboxEnumType<T>>();

            Type t = typeof(T);

            foreach (FieldInfo field in t.GetFields())
            {
                if (field.IsSpecialName == false)
                {
                    BindComboxEnumType<T> bind = new BindComboxEnumType<T>();
                    bind.Name = field.Name;
                    bind.TypeValue = field.GetRawConstantValue().ToString();
                    bindTypes.Add(bind);
                }

            }
        }

        public static List<BindComboxEnumType<T>> BindTypes
        {
            get
            {
                return bindTypes;
            }
        }
    }

    public static class ArrayExtensions
    {
        //public static void forEach(this Array array, Action<object> action)
        //{
        //    foreach (var item in array)
        //    {
        //        action(item);
        //    }
        //}
        //public static void forEach<T>(this IEnumerable<T> array, Action<T> action)
        //{
        //    foreach (var item in array)
        //    {
        //        action(item);
        //    }
        //}
    }

    public static class StringBuilderExtensions
    {
        public static void TrimStart(this  StringBuilder builder, char c)
        {
            TrimStart(builder, new[] { c });
        }
        public static void TrimStart(this  StringBuilder builder, params char[] c)
        {
            if (builder.Length == 0) return;
            int a = 0;
            while (c.Contains(builder[a]))
            {
                a++;
            }
            if (a > 0)
            {
                builder = builder.Remove(0, a > builder.Length ? builder.Length : a);
            }
        }
        public static void TrimEnd(this  StringBuilder builder, char c)
        {
            TrimEnd(builder, new[] { c });
        }
        public static void TrimEnd(this  StringBuilder builder, params char[] c)
        {
            if (builder.Length == 0) return;
            int a = builder.Length;
            while (c.Contains(builder[a]))
            {
                a--;
            }
            if (a < builder.Length)
            {
                builder = builder.Remove(a < 0 ? 0 : a, builder.Length - a);
            }
        }
        public static void Trim(this  StringBuilder builder, char c)
        {
            Trim(builder, new[] { c });
        }
        public static void Trim(this  StringBuilder builder, params char[] c)
        {
            TrimStart(builder, c);
            TrimEnd(builder, c);
        }
        public static void Concat(this  StringBuilder builder, params StringBuilder[] c)
        {
            foreach (var item in c)
            {
                if (!Object.ReferenceEquals(item, c))
                    builder.Append(c.ToString());
            }
        }
    }



    /// <summary>
    /// 将一个实体对象的值复制到另一个对象
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 引用类型的属性不支持克隆,wangyunpeng.2015-2-4补得注释。
        /// 本方法不是wangyunpeng写的。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Copy<T>(this object obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException("参数不能为null");
            T b = Activator.CreateInstance<T>();
            var properties = b.GetType().GetProperties().Where(p => p.PropertyType.IsPublic && p.CanWrite).ToList();
            var t = obj.GetType().GetProperties();
            foreach (var item in t)
            {
                var pro = properties.FirstOrDefault(x => x.Name == item.Name && x.PropertyType == item.PropertyType);
                if (pro != null)
                {
                    pro.SetValue(b, item.GetValue(obj, null), null);
                }
            }
            return b;
        }
        /// <summary>
        /// 对象克隆,wangyunpeng.2015-2-4.本方法是wangyunpeng写的。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj) where T : class
        {
            T clone = default(T);
            using (MemoryStream ms = new MemoryStream(1024))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                // 反序列化至另一个对象(即创建了一个原对象的深表副本)
                clone = (T)binaryFormatter.Deserialize(ms);
            }
            return clone;
        }
    }
    /// <summary>
    /// 将一个集合对象的值复制到另一个对象
    /// </summary>
    public static class IListExtensions
    {
        public static IList<T> CopyList<T>(this IEnumerable obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException("参数不能为null");
            Type tp = obj.GetType();
            if (!tp.IsGenericType) throw new ArgumentException("参数类型必须为泛型集合");
            ICollection collection = obj as ICollection;
            IList<T> list = new List<T>();
            foreach (var item in obj)
            {
                list.Add(item.Copy<T>());
            }
            return list;
        }

        /// <summary>
        /// 向上移动集合里面的某一个元素,wangyunpeng.2015-2-4.本方法是wangyunpeng写的。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要挪动的集合对象</param>
        /// <param name="index">挪动的位置</param>
        public static void Swap<T>(this List<T> list, int index)
        {
            if (index >= list.Count)
                return;

            T item1 = list[index];
            T item2 = list[index - 1];
            list.RemoveAt(index);
            list.RemoveAt(index - 1);

            list.Insert(index - 1, item1);
            list.Insert(index, item2);
        }
    }

    /// <summary>
    /// Decimal类型扩展
    /// add by mcj 2016年5月11日
    /// </summary>
    public static class DecimalExtensions
    {
        public static decimal ConvertBytes(this ulong b, int iteration)
        {
            long iter = 1;
            for (int i = 0; i < iteration; i++)
                iter *= 1024;
            return Math.Round((Convert.ToDecimal(b)) / Convert.ToDecimal(iter), 2, MidpointRounding.AwayFromZero);
        }
    }
}
