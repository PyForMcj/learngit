using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TLZ.Helper
{
    public static class IOHelper
    {
        #region 变量
        /// <summary>
        /// 目录分隔符
        /// </summary>
        public static readonly string DirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();
        /// <summary>
        /// 文件名中不能出现的字符数组
        /// </summary>
        public static readonly char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
        /// <summary>
        /// 目录名中不能出现的字符数组
        /// </summary>
        public static readonly char[] InvalidPathChars = System.IO.Path.GetInvalidPathChars();
        #endregion

        #region CombinePath
        /// <summary>
        /// 将多个字符串合并成一个目录字符串
        /// </summary>
        /// <param name="paths"></param>
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }
        #endregion

        #region GenerateFile
        /// <summary>
        /// GenerateFile 将字符串写成文件
        /// </summary>       
        public static void GenerateFile(string filePath, string fileContent)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(fileContent);
                    sw.Flush();
                }
            }
        }
        #endregion

        #region GetFileEncoding
        /// <summary>
        /// 得到文件编码方式
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Encoding GetFileEncoding(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return GetFileEncoding(fs);
            }
        }
        public static Encoding GetFileEncoding(FileStream fs)
        {
            /*byte[] Unicode=new byte[]{0xFF,0xFE};  
            byte[] UnicodeBIG=new byte[]{0xFE,0xFF};  
            byte[] UTF8=new byte[]{0xEF,0xBB,0xBF};*/

            byte[] ss = null;
            using (BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default))
            {
                ss = r.ReadBytes(3);
            }
            //编码类型 Coding=编码类型.ASCII;   
            if (ss != null && ss.Length > 0 && ss[0] >= 0xEF)
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }

        /// <summary>
        /// 得到HTML里面的Encoding
        /// </summary>
        /// <param name="htmlContent">HTML内容</param>
        /// <returns>null表示html内容中没有定义Encoding信息</returns>
        public static Encoding GetHtmEncoding(string htmlContent)
        {
            Encoding encoding = null;
            if (!string.IsNullOrWhiteSpace(htmlContent))
            {
                Match match = Regex.Match(htmlContent, @"<meta\s+http-equiv=""Content-Type""\s+content=""text/html;\s*charset=(?<charset>.*?)""\s*(/|)>", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    encoding = Encoding.GetEncoding(match.Groups["charset"].Value);
                }
                else
                {
                    match = Regex.Match(htmlContent, @"<meta\s+charset=""(?<charset>.*?)""\s*(/|)>", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        encoding = Encoding.GetEncoding(match.Groups["charset"].Value);
                    }
                }
            }
            return encoding;
        }
        #endregion

        #region GetFileContent
        /// <summary>
        /// GetFileContent 读取文本文件的内容
        /// </summary>       
        public static string GetFileContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string content = null;
            Encoding encoding = GetFileEncoding(filePath);
            using (StreamReader reader = new StreamReader(filePath, encoding))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }
        /// <summary>
        /// GetFileContent 读取文本文件的内容
        /// </summary>       
        public static string GetFileReadWriteContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string content = string.Empty;
            Encoding encoding = GetFileEncoding(filePath);
            using ( FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader streamReader = new StreamReader(fileStream, encoding);
                content = streamReader.ReadToEnd();
            }

            return content;
        }
        #endregion

        #region WriteBuffToFile
        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>    
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(buff, offset, len);
                    bw.Flush();
                }
            }
        }

        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>   
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.BaseStream.Position = 0;
                        bw.Write(buff);
                        bw.Flush();
                    }
                }
            }
        }
        #endregion

        #region ReadFileReturnBytes
        /// <summary>
        /// ReadFileReturnBytes 从文件中读取二进制数据
        /// </summary>      
        public static byte[] ReadFileReturnBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            byte[] result = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    result = br.ReadBytes((int)fs.Length);
                }
            }
            return result;
        }
        #endregion

        #region GetFileNameNoPath
        /// <summary>
        /// GetFileNameNoPath 获取不包括路径的文件名
        /// </summary>      
        public static string GetFileNameNoPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            return Path.GetFileName(filePath);
        }
        #endregion

        #region GetFileSize
        /// <summary>
        /// GetFileSize 获取目标文件的大小
        /// </summary>        
        public static long GetFileSize(string filePath)
        {
            long size = 0;
            if (!IsExistFilePath(filePath))
                return size;
            FileInfo info = new FileInfo(filePath);
            size = info.Length;
            return size;
        }
        /// <summary>
        /// GetFileSizeText 获取目标文件的大小
        /// </summary>
        /// <param name="size">字节</param>
        /// <returns></returns>
        public static string GetFileSizeText(double size)
        {
            if (size < 1024)
                return string.Format("{0} B", size.ToString("0.0"));
            else if (size < 1024 * 1024)
                return string.Format("{0} KB", (size / 1024.0f).ToString("0.0"));
            else if (size < 1024 * 1024 * 1024)
                return string.Format("{0} MB", (size / (1024.0f * 1024.0f)).ToString("0.0"));
            else
                return string.Format("{0} GB", (size / (1024.0f * 1024.0f * 1024.0f)).ToString("0.0"));
        }
        #endregion

        #region GetFileVersion
        /// <summary>
        /// 得到文件的版本信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileVersion(string filePath)
        {
            return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
        }
        #endregion

        #region GetFileNamesByPattern
        /// <summary>
        /// 获取指定目录名称里面,指定类型的文件列表
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static string[] GetFileNamesByPattern(string directoryPath, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string[] fileNames = null;
            if (Directory.Exists(directoryPath))
            {
                fileNames = Directory.GetFiles(directoryPath, searchPattern, searchOption);
            }
            return fileNames;
        }
        #endregion

        #region ReadFileData
        /// <summary>
        /// ReadFileData 从文件流中读取指定大小的内容
        /// </summary>       
        public static void ReadFileData(FileStream fs, byte[] buff, int count, int offset)
        {
            int readCount = 0;
            while (readCount < count)
            {
                int read = fs.Read(buff, offset + readCount, count - readCount);
                readCount += read;
            }

            return;
        }
        #endregion

        #region GetFileDirectory
        /// <summary>
        /// GetFileDirectory 获取文件所在的目录路径
        /// </summary>       
        public static string GetFileDirectory(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
        #endregion

        #region DeleteFile
        /// <summary>
        /// DeleteFile 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                }
            }
        }
        #endregion

        #region EnsureExtendName
        /// <summary>
        /// EnsureExtendName 确保扩展名正确
        /// </summary>       
        public static string EnsureExtendName(string origin_path, string extend_name)
        {
            if (Path.GetExtension(origin_path) != extend_name)
            {
                origin_path += extend_name;
            }

            return origin_path;
        }
        #endregion

        #region ClearDirectory
        /// <summary>
        /// 删除目录里面的所有文件（包括子目录和子目录里面的文件,不能删除当前目录本身）
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="searchPattern"></param>
        public static void ClearDirectory(string dir, bool isDeleteChildDir = false, string searchPattern = "*.*")
        {
            if (Directory.Exists(dir))
            {
                foreach (string item in Directory.GetFileSystemEntries(dir, searchPattern, SearchOption.TopDirectoryOnly))
                {
                    if (File.Exists(item))
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        DeleteDirectory(item, isDeleteChildDir);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取某一磁盘目录里面所有满足搜索条件的文件
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="searchPattern">搜索条件</param>
        /// <param name="option">是否包含子目录里面的文件</param>
        /// <returns>返回文件信息数组，null表示目录名不存在</returns>
        public static FileInfo[] GetFileInfos(string path, string searchPattern = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            FileInfo[] fileInfos = null;
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                fileInfos = directoryInfo.GetFiles(searchPattern, option);
            }
            return fileInfos;
        }
        /// <summary>
        /// 获得文件的修改时间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DateTime GetFileModifyDateTime(string path)
        {
            DateTime result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(path))
                return result;
            if (!File.Exists(path))
                return result;
            FileInfo info = new FileInfo(path);
            result = info.LastWriteTime;
            return result;
        }
        /// <summary>
        /// 得到文件的创建时间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DateTime GetFileCreateDateTime(string path)
        {
            DateTime result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(path))
                return result;
            if (!File.Exists(path))
                return result;
            FileInfo info = new FileInfo(path);
            result = info.CreationTime;
            return result;
        }
        /// <summary>
        /// 创建时间和修改时间那个最近取哪个时间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DateTime GetFileMaxDateTime(string path)
        {
            DateTime result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(path))
                return result;
            if (!File.Exists(path))
                return result;
            FileInfo info = new FileInfo(path);
            result = info.CreationTime > info.LastWriteTime ? info.CreationTime : info.LastWriteTime;
            return result;
        }

        /// <summary>
        /// 得到当前文件所在的目录名
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetDirectoryName(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;
            return fileInfo.DirectoryName;
        }

        /// <summary>
        /// 得到当前文件所在的目录名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            return Path.GetDirectoryName(filePath);
        }

        /// <summary>
        /// 获取当前目录的子目录名，不包含子目录里面的目录名称
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetChildDirectory(string dirPath)
        {
            List<string> childDirectoryList = null;
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo[] directoryInfos = new DirectoryInfo(dirPath).GetDirectories();
                childDirectoryList = new List<string>(directoryInfos.Length);
                foreach (DirectoryInfo directoryInfo in directoryInfos)
                {
                    childDirectoryList.Add(directoryInfo.Name);
                }
                Array.Clear(directoryInfos, 0, directoryInfos.Length);
                directoryInfos = null;
            }
            return childDirectoryList;
        }

        /// <summary>
        /// 得到末尾带有反斜杠的目录字符串
        /// </summary>
        /// <param name="dirPath">目录字符串</param>
        /// <returns>末尾带有反斜杠的目录字符串</returns>
        public static string GetWithBackslash(string dirPath)
        {
            if (!dirPath.EndsWith(DirectorySeparatorChar))
            {
                dirPath += DirectorySeparatorChar;
            }
            return dirPath;
        }

        /// <summary>
        /// 得到文件的名称（包含扩展名）
        /// </summary>
        /// <param name="filePath">文件目录字符串</param>
        /// <returns></returns>
        public static string GetFileNameWithExtension(string filePath)
        {
            return GetFileNameNoPath(filePath);
        }

        /// <summary>
        /// 得到文件名称（不包含扩展名）
        /// </summary>
        /// <param name="filePath">文件目录字符串</param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 得到文件的扩展名（开头包含“.”）
        /// </summary>
        /// <param name="filePath">文件目录字符串</param>
        /// <returns></returns>
        public static string GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// 判断目录是否存在，如果不存在就创建目录，如果存在不执行任何操作
        /// </summary>
        /// <param name="dirPath"></param>
        public static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="descPath"></param>
        public static void CopyFile(string sourcePath, string descPath)
        {
            if (File.Exists(sourcePath))
            {
                if (File.Exists(descPath))
                {
                    File.Delete(descPath);
                }
                string descDir = Path.GetDirectoryName(descPath);
                if (!Directory.Exists(descDir))
                {
                    Directory.CreateDirectory(descDir);
                }
                File.Copy(sourcePath, descPath, true);
            }
        }

        /// <summary>
        /// 剪切文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="descPath"></param>
        public static void MoveFile(string sourcePath, string descPath)
        {
            if (File.Exists(sourcePath))
            {
                if (File.Exists(descPath))
                {
                    File.Delete(descPath);
                }
                string descDir = Path.GetDirectoryName(descPath);
                if (!Directory.Exists(descDir))
                {
                    Directory.CreateDirectory(descDir);
                }
                File.Move(sourcePath, descPath);
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExistFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            return File.Exists(filePath);
        }
        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool IsExistDirectoryPath(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return false;
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// 删除指定目录中的所有文件和目录中子目录的所有文件(支持删除目录名的操作)
        /// 解决删除目录提示：System.IO.IOException: 目录不是空的。  
        /// 删除一个目录，先遍历删除其下所有文件和目录（递归）
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isDeleteChildDir">是否删除dir目录中的子目录名</param>
        public static void DeleteDirectory(string dir, bool isDeleteChildDir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    //先删除该目录下的文件  
                    string[] strTemps = Directory.GetFiles(dir);
                    foreach (string str in strTemps)
                    {
                        File.Delete(str);
                    }
                    Array.Clear(strTemps, 0, strTemps.Length);
                    strTemps = null;
                    //删除子目录，递归  
                    strTemps = Directory.GetDirectories(dir);
                    foreach (string str in strTemps)
                    {
                        DeleteDirectory(str, isDeleteChildDir);
                    }
                    if (isDeleteChildDir)
                    {
                        //删除该目录  
                        Directory.Delete(dir);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 读取嵌入的资源文件
        /// </summary>
        /// <param name="assemblyName">程序集完整名称</param>
        /// <param name="resourceName">资源的完整名称</param>
        /// <returns></returns>
        public static Stream ReadEmbeddedResources(string assemblyName, string resourceName)
        {
            Stream result = null;
            if (string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(resourceName))
                return result;
            try
            {
                Assembly ab = Assembly.Load(assemblyName);
                if (ab != null)
                {
                    result = ab.GetManifestResourceStream(resourceName);
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
        /// 如果文件存在,自动更名(1)...(n)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string AutoRenFileName(string fileName)
        {
            string s = fileName;
            int i = 1;
        fileIsExistNameAutoRenFileName:
            if (File.Exists(s))
            {
                int index = fileName.LastIndexOf('.');
                s = fileName.Insert(index, "(" + i + ")");
                i++;
                goto fileIsExistNameAutoRenFileName;
            }
            return s;
        }
    }
}
