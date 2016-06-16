using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TLZ.Helper
{/// <summary>
    /// 消息报文加密、解密器
    /// </summary>
    public static class EncryptHelper
    {
        private static readonly SymmetricAlgorithm _CryptoService;
        private static readonly byte[] _Key;
        private static readonly byte[] _IV;
        private const int COUNT = 1024;

        static EncryptHelper()
        {
            _CryptoService = new RijndaelManaged();

            string sKey = "E2ghj*Ghg7!rNIfb&95GUY86GfahUb#w";//长度固定为32位byte[]
            string sIV = "ASDF7031KL4J01^&";//长度固定为16位byte[]

            //以下代码是判断Key和IV的长度 如果过长则截取 过短则补足 满足key为32位byte[] iv为16位byte[]
            _CryptoService.GenerateKey();
            int keyLength = _CryptoService.Key.Length;
            if (sKey.Length > keyLength)
                sKey = sKey.Substring(0, keyLength);
            else if (sKey.Length < keyLength)
                sKey = sKey.PadRight(keyLength, ' ');
            _Key = ASCIIEncoding.ASCII.GetBytes(sKey);

            _CryptoService.GenerateIV();
            int ivLength = _CryptoService.IV.Length;
            if (sIV.Length > ivLength)
                sIV = sIV.Substring(0, ivLength);
            else if (sIV.Length < ivLength)
                sIV = sIV.PadRight(ivLength, ' ');
            _IV = ASCIIEncoding.ASCII.GetBytes(sIV);

            _CryptoService.Key = _Key;
            _CryptoService.IV = _IV;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="text">明文字符串</param>
        /// <returns>密文字符串</returns>
        public static string Encrypt(string text)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(text))
                return result;
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] eData = Encrypt(data);
            result = Convert.ToBase64String(eData);
            Array.Clear(eData, 0, eData.Length);
            return result;
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="text">密文字符串</param>
        /// <returns>明文字符串</returns>
        public static string Decrypt(string text)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(text))
                return result;
            byte[] data = Convert.FromBase64String(text);
            byte[] eData = Decrypt(data);
            result = Encoding.UTF8.GetString(eData);
            Array.Clear(eData, 0, eData.Length);
            return result;
        }
        /// <summary>
        /// 加密字节
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="isClearData">加密后是否清除明文里面的数据</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] data, bool isClearData = true)
        {
            byte[] result = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform encrypto = _CryptoService.CreateEncryptor())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                }
                result = ms.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return result;
        }

        /// <summary>
        /// 解密字节
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="isClearData">解密后是否清除密文里面的数据</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] data, bool isClearData = true)
        {
            byte[] result = null;
            using (MemoryStream o = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data, 0, data.Length))
                {
                    using (ICryptoTransform encrypto = _CryptoService.CreateDecryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read))
                        {
                            cs.Flush();
                            int size = 0;
                            byte[] buffer = new byte[COUNT];
                            while ((size = cs.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                o.Write(buffer, 0, size);
                            }
                        }
                    }
                }
                result = o.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return result;
        }

        /// <summary>
        /// 文件MD5加密
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileMd5(string filePath)
        {
            StringBuilder sTemp = new StringBuilder("");
            if (File.Exists(filePath))
            {
                byte[] md5Hashs = null;
                try
                {
                    md5Hashs = File.ReadAllBytes(filePath);
                    if (md5Hashs != null && md5Hashs.Length > 0)
                    {
                        using (MD5 md5 = System.Security.Cryptography.MD5.Create())
                        {
                            md5Hashs = md5.ComputeHash(md5Hashs);
                        }
                        if (md5Hashs != null)
                        {
                            for (int i = 0; i < md5Hashs.Length; i++)
                            {
                                sTemp.Append(md5Hashs[i].ToString("X2"));
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    sTemp = new StringBuilder("");
                }
                finally
                {
                    if (md5Hashs != null)
                    {
                        if (md5Hashs.Length > 0)
                        {
                            Array.Clear(md5Hashs, 0, md5Hashs.Length);
                        }
                        md5Hashs = null;
                    }
                }
            }
            return sTemp.ToString();
        }

        /// <summary>
        /// 自定义一个简单的对称式加密解密算法 执行一次加密，两次解密 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string CustomEncrypt(string plainText)
        {
            char[] plainTexts = plainText.ToCharArray();
            for (int i = 0; i < plainTexts.Length; i++)
            {
                plainTexts[i] = (char)(plainTexts[i] ^ 't');
            }
            return new string(plainTexts);
        }
    }
}
