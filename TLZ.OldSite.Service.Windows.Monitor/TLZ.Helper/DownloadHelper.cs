using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace TLZ.Helper
{
    public static class DownloadHelper
    {
        /// <summary>
        /// 3850是Response流一次读取的最大字节数
        /// </summary>
        private const int RESPONSE_STREAM_MAX_LENGTH = 3850;
        /// <summary>
        /// 下载文件并保存到本地目录。
        /// </summary>
        /// <param name="url">下载资源url</param>
        /// <param name="filePath">存放下载文件完整路径（包括文件名）</param>
        /// <returns>true表示下载成功，false表示下载失败</returns>
        public static bool DownloadFile(string url, string filePath)
        {
            return DownloadFile(url, filePath, null);
        }

        /// <summary>
        /// 下载文件并保存到本地目录。
        /// 该方法用在下载Update.xml文件。
        /// </summary>
        /// <param name="url">下载资源url</param>
        /// <param name="filePath">存放下载文件完整路径（包括文件名）</param>
        /// <param name="webProxy">Web代理</param>
        /// <returns>true表示下载成功，false表示下载失败</returns>
        public static bool DownloadFile(string url, string filePath, IWebProxy webProxy)
        {
            bool flag = false;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                if (webProxy == null)
                {
                    webRequest.Proxy = HttpWebRequest.DefaultWebProxy;
                }
                else
                {
                    webRequest.Proxy = webProxy;
                }
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        string dirPath = IOHelper.GetFileDirectory(filePath);
                        IOHelper.CreateDirectory(dirPath);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            byte[] bs = new byte[RESPONSE_STREAM_MAX_LENGTH];
                            int size = 0;
                            while ((size = responseStream.Read(bs, 0, RESPONSE_STREAM_MAX_LENGTH)) > 0)
                            {
                                fileStream.Write(bs, 0, size);
                            }
                            fileStream.Flush();
                        }
                    }
                    flag = true;
                }
                webRequest = null;
            }
            catch (WebException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (Exception)
            {
            }
            return flag;
        }
    }
}
