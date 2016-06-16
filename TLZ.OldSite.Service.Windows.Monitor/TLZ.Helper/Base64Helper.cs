using System;
using System.Text;

namespace TLZ.Helper
{
    /// <summary> 
    /// 实现Base64加密解密 
    /// </summary> 
    /// <remarks>
    /// [2013-12-14] Develop by SunLiang.
    /// </remarks>
    public static class Base64Helper
    {
        /// <summary> 
        /// Base64加密 
        /// </summary> 
        /// <param name="codeName">加密采用的编码方式</param> 
        /// <param name="source">待加密的明文</param> 
        /// <returns></returns> 
        public static string Encode(string source, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(source);
            try
            {
                return  Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 
        /// Base64加密，采用utf8编码方式加密 
        /// </summary> 
        /// <param name="source">待加密的明文</param> 
        /// <returns>加密后的字符串</returns> 
        public static string Encode(string source)
        {
            return Encode( source,Encoding.UTF8);
        }

        /// <summary> 
        /// Base64解密 
        /// </summary> 
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param> 
        /// <param name="result">待解密的密文</param> 
        /// <returns>解密后的字符串</returns> 
        public static string Decode(string result,Encoding encode)
        {
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                return encode.GetString(bytes);
            }
            catch
            {
                return null ;
            }
        }

        /// <summary> 
        /// Base64解密，采用utf8编码方式解密 
        /// </summary> 
        /// <param name="result">待解密的密文</param> 
        /// <returns>解密后的字符串</returns> 
        public static string Decode(string result)
        {
            return Decode(result,Encoding.UTF8);
        }
    } 
}
