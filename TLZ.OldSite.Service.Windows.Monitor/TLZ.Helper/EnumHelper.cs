using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
namespace TLZ.Helper
{
    public class EnumHelper
    {
        /// <summary>
        /// 通过枚举值获取枚举类型的Name
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static string GetName(Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }
        /// <summary>
        /// 获得枚举描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            string result = null;
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute; ;
            result = attribute != null ? attribute.Description : value.ToString();
            return result;
        }
        /// <summary>
        /// 获取枚举类型的数组的描述列表
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enums"></param>
        /// <returns></returns>
        public static List<string> GetEnumDescriptionList<TEnum>(TEnum[] enums) where TEnum : struct
        {
            Array array = Enum.GetValues(typeof(TEnum));
            List<string> descriptionList = new List<string>(array.Length);
            string description = null;
            foreach (object value in array)
            {
                foreach (TEnum item in enums)
                {
                    if (item.Equals(value))
                    {
                        description = GetEnumDescription((Enum)value);
                        descriptionList.Add(description);
                    }
                }
            }
            descriptionList.TrimExcess();
            return descriptionList;
        }
        /// <summary>
        /// 获得枚举描述/值集合
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> GetDescriptionValueDict<TEnum>() where TEnum : struct
        {
            Array array = Enum.GetValues(typeof(TEnum));
            Dictionary<string, int> dict = new Dictionary<string, int>(array.Length);
            string description = null;
            foreach (object value in array)
            {
                description = GetEnumDescription((Enum)value);
                dict.Add(description, (int)value);
            }
            Array.Clear(array, 0, array.Length);
            array = null;
            return dict;
        }

        /// <summary>
        /// 获得枚举Key/Value集合
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> GetKeyValueDict<TEnum>() where TEnum : struct
        {
            Array array = Enum.GetValues(typeof(TEnum));
            Dictionary<string, int> dict = new Dictionary<string, int>(array.Length);
            string valueString = null;
            foreach (object value in array)
            {
                valueString = value.ToString();
                dict.Add(valueString, (int)value);
            }
            Array.Clear(array, 0, array.Length);
            array = null;
            return dict;
        }
        /// <summary>
        /// 获得枚举Key/Value集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetKeyValueDict(Type type)
        {
            if (!type.IsEnum)
            {
                throw new Exception("type必须为枚举类型");
            }
            Array array = Enum.GetValues(type);
            Dictionary<string, int> dict = new Dictionary<string, int>(array.Length);
            string valueString = null;
            foreach (object value in array)
            {
                valueString = value.ToString();
                dict.Add(valueString, (int)value);
            }
            Array.Clear(array, 0, array.Length);
            array = null;
            return dict;
        }
        /// <summary>
        /// 获得枚举[Key/Value]JSON对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetKeyValueJson(Type type)
        {
            return JsonHelper.ConvertJsonToStr(GetKeyValueDict(type));
        }

      
        /// <summary>
        /// 返回枚举类型的数组（返回Enum类型的数组）
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Array GetValues(Type enumType)
        {
            return Enum.GetValues(enumType);
        }

        /// <summary>
        /// 得到枚举类型所有值的数组（返回Int32类型的数组）
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static int[] GetEnumValues<TEnum>() where TEnum : struct
        {
            int[] array = null;
            try
            {
                array = Enum.GetValues(typeof(TEnum)).Cast<int>().ToArray();
            }
            catch
            {
                array = null;
            }
            return array;
        }
        /// <summary>
        /// 得到枚举类型所有值的数组（返回Int32类型的数组）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetEnumValues(Type type)
        {
            int[] array = null;
            if (!type.IsEnum)
            {
                throw new Exception("type必须为枚举类型");
            }
            try
            {
                array = Enum.GetValues(type).Cast<int>().ToArray();
            }
            catch
            {
                array = null;
            }
            return array;
        }
        /// <summary>
        /// 得到枚举类型所有值的数组（返回TEnum类型的数组）
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns></returns>
        public static TEnum[] GetEnumTypes<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
        }
    }
}
