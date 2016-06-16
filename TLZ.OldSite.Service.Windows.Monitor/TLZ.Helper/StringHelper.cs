#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：YANGHUANWEN 
     * 文件名：  StringHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2014/6/13 11:34:08 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/6/13 11:34:08 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TLZ.Helper
{
    /// <summary>
    /// 从NHibernate中取出来的类型。
    /// </summary>
    public static class StringHelper
    {
        public const string WhiteSpace = " \n\r\f\t";

        /// <summary></summary>
        public const char Dot = '.';

        /// <summary></summary>
        public const char Underscore = '_';

        /// <summary></summary>
        public const string CommaSpace = ", ";

        /// <summary></summary>
        public const string Comma = ",";

        /// <summary></summary>
        public const string OpenParen = "(";

        /// <summary></summary>
        public const string ClosedParen = ")";

        /// <summary></summary>
        public const char SingleQuote = '\'';

        /// <summary></summary>
        public const string SqlParameter = "?";

        public const int AliasTruncateLength = 10;

        public static string Join(string separator, IEnumerable objects)
        {
            StringBuilder buf = new StringBuilder();
            bool first = true;

            foreach (object obj in objects)
            {
                if (!first)
                {
                    buf.Append(separator);
                }

                first = false;
                buf.Append(obj);
            }

            return buf.ToString();
        }

        internal static string Join<T>(string separator, IEnumerable<T> objects)
        {
            return string.Join(separator, objects);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(string str, int times)
        {
            StringBuilder buf = new StringBuilder(str.Length * times);
            for (int i = 0; i < times; i++)
            {
                buf.Append(str);
            }
            return buf.ToString();
        }

        public static string Replace(string template, string placeholder, string replacement)
        {
            return Replace(template, placeholder, replacement, false);
        }

        public static string Replace(string template, string placeholder, string replacement, bool wholeWords)
        {
            // sometimes a null value will get passed in here -> SqlWhereStrings are a good example
            if (template == null)
            {
                return null;
            }

            int loc = template.IndexOf(placeholder);
            if (loc < 0)
            {
                return template;
            }
            else
            {
                // NH different implementation (NH-1253)
                string replaceWith = replacement;
                if (loc + placeholder.Length < template.Length)
                {
                    string afterPlaceholder = template[loc + placeholder.Length].ToString();
                    //After a token in HQL there can be whitespace, closedparen or comma.. 
                    if (wholeWords && !(WhiteSpace.Contains(afterPlaceholder) || ClosedParen.Equals(afterPlaceholder) || Comma.Equals(afterPlaceholder)))
                    {
                        //If this is not a full token we don't want to touch it
                        replaceWith = placeholder;
                    }
                }

                return
                    new StringBuilder(template.Substring(0, loc)).Append(replaceWith).Append(
                        Replace(template.Substring(loc + placeholder.Length), placeholder, replacement, wholeWords)).ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="placeholder"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceOnce(string template, string placeholder, string replacement)
        {
            int loc = template.IndexOf(placeholder);
            if (loc < 0)
            {
                return template;
            }
            else
            {
                return new StringBuilder(template.Substring(0, loc))
                    .Append(replacement)
                    .Append(template.Substring(loc + placeholder.Length))
                    .ToString();
            }
        }

        /// <summary>
        /// Just a fa鏰de for calling string.Split()
        /// We don't use our StringTokenizer because string.Split() is
        /// more efficient (but it only works when we don't want to retrieve the delimiters)
        /// </summary>
        /// <param name="separators">separators for the tokens of the list</param>
        /// <param name="list">the string that will be broken into tokens</param>
        /// <returns></returns>
        public static string[] Split(string separators, string list)
        {
            return list.Split(separators.ToCharArray());
        }

        /// <summary>
        /// Splits the String using the StringTokenizer.  
        /// </summary>
        /// <param name="separators">separators for the tokens of the list</param>
        /// <param name="list">the string that will be broken into tokens</param>
        /// <param name="include">true to include the separators in the tokens.</param>
        /// <returns></returns>
        /// <remarks>
        /// This is more powerful than Split because you have the option of including or 
        /// not including the separators in the tokens.
        /// </remarks>
        public static string[] Split(string separators, string list, bool include)
        {
            var tokens = new StringTokenizer(list, separators, include);
            return tokens.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static string Unqualify(string qualifiedName)
        {
            if (qualifiedName.IndexOf('`') > 0)
            {
                // less performance but correctly manage generics classes
                // where the entity-name was not specified
                // Note: the enitty-name is mandatory when the user want work with different type-args
                // for the same generic-entity implementation
                return GetClassname(qualifiedName);
            }
            return Unqualify(qualifiedName, ".");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Unqualify(string qualifiedName, string seperator)
        {
            return qualifiedName.Substring(qualifiedName.LastIndexOf(seperator) + 1);
        }

        /// <summary>
        /// Takes a fully qualified type name and returns the full name of the 
        /// Class - includes namespaces.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetFullClassname(string typeName)
        {
            return new TypeNameParser(null, null).ParseTypeName(typeName).Type;
        }

        /// <summary>
        /// Takes a fully qualified type name (can include the assembly) and just returns
        /// the name of the Class.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetClassname(string typeName)
        {
            //string[] splitClassname = GetFullClassname(typeName).Split('.');
            string fullClassName = GetFullClassname(typeName);

            int genericTick = fullClassName.IndexOf('`');
            if (genericTick != -1)
            {
                string nameBeforeGenericTick = fullClassName.Substring(0, genericTick);
                int lastPeriod = nameBeforeGenericTick.LastIndexOf('.');
                return lastPeriod != -1 ? fullClassName.Substring(lastPeriod + 1) : fullClassName;
            }
            string[] splitClassname = fullClassName.Split('.');
            return splitClassname[splitClassname.Length - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static string Qualifier(string qualifiedName)
        {
            int loc = qualifiedName.LastIndexOf(".");
            if (loc < 0)
            {
                return String.Empty;
            }
            else
            {
                return qualifiedName.Substring(0, loc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string[] Suffix(string[] columns, string suffix)
        {
            if (suffix == null)
            {
                return columns;
            }
            string[] qualified = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                qualified[i] = Suffix(columns[i], suffix);
            }
            return qualified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string Suffix(string name, string suffix)
        {
            return (suffix == null) ?
                   name :
                   name + suffix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string[] Prefix(string[] columns, string prefix)
        {
            if (prefix == null)
            {
                return columns;
            }
            string[] qualified = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                qualified[i] = prefix + columns[i];
            }
            return qualified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static string Root(string qualifiedName)
        {
            int loc = qualifiedName.IndexOf(".");
            return (loc < 0)
                    ? qualifiedName
                    : qualifiedName.Substring(0, loc);
        }

        /// <summary>
        /// Converts a <see cref="String"/> in the format of "true", "t", "false", or "f" to
        /// a <see cref="Boolean"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// The <c>value</c> converted to a <see cref="Boolean"/> .
        /// </returns>
        public static bool BooleanValue(string value)
        {
            string trimmed = value.Trim().ToLowerInvariant();
            return trimmed.Equals("true") || trimmed.Equals("t");
        }

        private static string NullSafeToString(object obj)
        {
            return obj == null ? "(null)" : obj.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToString(object[] array)
        {
            int len = array.Length;

            // if there is no value in the array then return no string...
            if (len == 0)
            {
                return String.Empty;
            }

            StringBuilder buf = new StringBuilder(len * 12);
            for (int i = 0; i < len - 1; i++)
            {
                buf.Append(NullSafeToString(array[i])).Append(CommaSpace);
            }
            return buf.Append(NullSafeToString(array[len - 1])).ToString();
        }

        public static string LinesToString(this string[] text)
        {
            if (text == null)
            {
                return null;
            }
            if (text.Length == 1)
            {
                return text[0];
            }
            var sb = new StringBuilder(200);
            Array.ForEach(text, t => sb.AppendLine(t));
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="placeholders"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string[] Multiply(string str, IEnumerator placeholders, IEnumerator replacements)
        {
            string[] result = new string[] { str };
            while (placeholders.MoveNext())
            {
                replacements.MoveNext();
                result = Multiply(result, placeholders.Current as string, replacements.Current as string[]);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="placeholder"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string[] Multiply(string[] strings, string placeholder, string[] replacements)
        {
            string[] results = new string[replacements.Length * strings.Length];
            int n = 0;
            for (int i = 0; i < replacements.Length; i++)
            {
                for (int j = 0; j < strings.Length; j++)
                {
                    results[n++] = ReplaceOnce(strings[j], placeholder, replacements[i]);
                }
            }
            return results;
        }

        /// <summary>
        /// Counts the unquoted instances of the character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int CountUnquoted(string str, char character)
        {
            if (SingleQuote == character)
            {
                throw new ArgumentOutOfRangeException("character", "Unquoted count of quotes is invalid");
            }

            // Impl note: takes advantage of the fact that an escaped single quote
            // embedded within a quote-block can really be handled as two separate
            // quote-blocks for the purposes of this method...
            int count = 0;
            char[] chars = str.ToCharArray();
            int stringLength = string.IsNullOrEmpty(str) ? 0 : chars.Length;
            bool inQuote = false;
            for (int indx = 0; indx < stringLength; indx++)
            {
                if (inQuote)
                {
                    if (SingleQuote == chars[indx])
                    {
                        inQuote = false;
                    }
                }
                else if (SingleQuote == chars[indx])
                {
                    inQuote = true;
                }
                else if (chars[indx] == character)
                {
                    count++;
                }
            }
            return count;
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotEmpty(string str)
        {
            return !IsEmpty(str);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Qualify(string prefix, string name)
        {
            char first = name[0];

            // Should we check for prefix == string.Empty rather than a length check?
            if (!string.IsNullOrEmpty(prefix) && first != SingleQuote && !char.IsDigit(first))
            {
                return prefix + Dot + name;
            }
            else
            {
                return name;
            }
        }

        public static string[] Qualify(string prefix, string[] names)
        {
            // Should we check for prefix == string.Empty rather than a length check?
            if (!string.IsNullOrEmpty(prefix))
            {
                int len = names.Length;
                string[] qualified = new string[len];
                for (int i = 0; i < len; i++)
                {
                    qualified[i] = names[i] == null ? null : Qualify(prefix, names[i]);
                }
                return qualified;
            }
            else
            {
                return names;
            }
        }

        public static int FirstIndexOfChar(string sqlString, string str, int startIndex)
        {
            return sqlString.IndexOfAny(str.ToCharArray(), startIndex);
        }

        public static string Truncate(string str, int length)
        {
            if (str.Length <= length)
            {
                return str;
            }
            else
            {
                return str.Substring(0, length);
            }
        }

        public static int LastIndexOfLetter(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetter(str, i) /*&& !('_'==character)*/)
                {
                    return i - 1;
                }
            }
            return str.Length - 1;
        }

        public static string UnqualifyEntityName(string entityName)
        {
            string result = Unqualify(entityName);
            int slashPos = result.IndexOf('/');
            if (slashPos > 0)
            {
                result = result.Substring(0, slashPos - 1);
            }
            return result;
        }

        public static string GenerateAlias(string description)
        {
            return GenerateAliasRoot(description) + Underscore;
        }

        /// <summary>
        /// Generate a nice alias for the given class name or collection role
        /// name and unique integer. Subclasses do <em>not</em> have to use
        /// aliases of this form.
        /// </summary>
        /// <returns>an alias of the form <c>foo1_</c></returns>
        public static string GenerateAlias(string description, int unique)
        {
            return GenerateAliasRoot(description) +
                   unique +
                   Underscore;
        }

        private static string GenerateAliasRoot(string description)
        {
            // remove any generic arguments attached to description
            int indexOfBacktick = description.IndexOf('`');
            if (indexOfBacktick > 0)
            {
                description = Truncate(description, indexOfBacktick);
            }

            string result = Truncate(UnqualifyEntityName(description), AliasTruncateLength)
                .ToLowerInvariant()
                .Replace('/', '_') // entityNames may now include slashes for the representations
                .Replace('+', '_') // classname may be an inner class
                .Replace('[', '_') // classname may contain brackets
                .Replace(']', '_')
                .Replace('`', '_') // classname may contain backticks (generic types)
                .TrimStart('_') // remove underscores from the beginning of the alias (for Firebird).
                ;

            if (char.IsDigit(result, result.Length - 1))
            {
                return result + "x"; //ick!
            }
            if (char.IsLetter(result[0]) || '_' == result[0])
            {
                return result;
            }
            return "alias_" + result;
        }

        public static string MoveAndToBeginning(string filter)
        {
            if (filter.Trim().Length > 0)
            {
                filter += " and ";
                if (filter.StartsWith(" and "))
                {
                    filter = filter.Substring(4);
                }
            }
            return filter;
        }

        public static string Unroot(string qualifiedName)
        {
            int loc = qualifiedName.IndexOf(".");
            return (loc < 0) ? qualifiedName : qualifiedName.Substring(loc + 1);
        }

        public static bool EqualsCaseInsensitive(string a, string b)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(a, b) == 0;
        }

        public static int IndexOfCaseInsensitive(string source, string value)
        {
            return source.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int IndexOfCaseInsensitive(string source, string value, int startIndex)
        {
            return source.IndexOf(value, startIndex, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int IndexOfCaseInsensitive(string source, string value, int startIndex, int count)
        {
            return source.IndexOf(value, startIndex, count, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int LastIndexOfCaseInsensitive(string source, string value)
        {
            return source.LastIndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithCaseInsensitive(string source, string prefix)
        {
            return source.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the interned string equal to <paramref name="str"/> if there is one, or <paramref name="str"/>
        /// otherwise.
        /// </summary>
        /// <param name="str">A <see cref="string" /></param>
        /// <returns>A <see cref="string" /></returns>
        public static string InternedIfPossible(string str)
        {
            if (str == null)
            {
                return null;
            }

            string interned = string.IsInterned(str);
            if (interned != null)
            {
                return interned;
            }

            return str;
        }

        public static string CollectionToString(IEnumerable keys)
        {
            var sb = new StringBuilder();
            foreach (object o in keys)
            {
                sb.Append(o);
                sb.Append(", ");
            }
            if (sb.Length != 0)//remove last ", "
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        /// <summary>
        /// 将指定的字符串转换为大写。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperCase(string str)
        {
            return str == null ? null : str.ToUpperInvariant();
        }
        /// <summary>
        /// 将指定的字符串转换为小写。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLowerCase(string str)
        {
            return str == null ? null : str.ToLowerInvariant();
        }
        /// <summary>
        /// 将指定的字符串转换为词首字母大写。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            System.Globalization.TextInfo textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
            //System.Globalization.TextInfo textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str);
        }

        public static bool IsBackticksEnclosed(string identifier)
        {
            return !string.IsNullOrEmpty(identifier) && identifier.StartsWith("`") && identifier.EndsWith("`");
        }

        public static string PurgeBackticksEnclosing(string identifier)
        {
            if (IsBackticksEnclosed(identifier))
            {
                return identifier.Substring(1, identifier.Length - 2);
            }
            return identifier;
        }

        public static string[] ParseFilterParameterName(string filterParameterName)
        {
            int dot = filterParameterName.IndexOf(".");
            if (dot <= 0)
            {
                throw new ArgumentException("Invalid filter-parameter name format; the name should be a property path.", "filterParameterName");
            }
            string filterName = filterParameterName.Substring(0, dot);
            string parameterName = filterParameterName.Substring(dot + 1);
            return new[] { filterName, parameterName };
        }
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Compress(string text)
        {
            string compressedText = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        zip.Write(bytes, 0, bytes.Length);
                    }
                    ms.Position = 0;
                    byte[] compresseds = new byte[ms.Length];
                    ms.Read(compresseds, 0, compresseds.Length);
                    byte[] buffers = new byte[compresseds.Length + 4];
                    System.Buffer.BlockCopy(compresseds, 0, buffers, 4, compresseds.Length);
                    Array.Clear(compresseds, 0, compresseds.Length);
                    compresseds = null;
                    System.Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, buffers, 0, 4);
                    compressedText = Convert.ToBase64String(buffers);
                    Array.Clear(buffers, 0, buffers.Length);
                    buffers = null;
                }
                Array.Clear(bytes, 0, bytes.Length);
                bytes = null;
            }
            return compressedText;
        }
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="compressedText"></param>
        /// <returns></returns>
        public static string Decompress(string compressedText)
        {
            string decompressText = null;
            if (!string.IsNullOrWhiteSpace(compressedText))
            {
                byte[] bytes = Convert.FromBase64String(compressedText);
                using (MemoryStream ms = new MemoryStream())
                {
                    int msgLength = BitConverter.ToInt32(bytes, 0);
                    ms.Write(bytes, 4, bytes.Length - 4);
                    byte[] buffer = new byte[msgLength];
                    ms.Position = 0;
                    using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        zip.Read(buffer, 0, buffer.Length);
                    }
                    decompressText = Encoding.UTF8.GetString(buffer);
                }
                Array.Clear(bytes, 0, bytes.Length);
                bytes = null;
            }
            return decompressText;
        }

        /// <summary>
        /// 短URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] ShortUrl(string url)
        {
            //可以自定义生成MD5加密字符传前的混合KEY
            //要使用生成URL的字符
            string[] chars = new string[]{
              "a","b","c","d","e","f","g","h",
              "i","j","k","l","m","n","o","p",
              "q","r","s","t","u","v","w","x",
              "y","z","0","1","2","3","4","5",
              "6","7","8","9","A","B","C","D",
              "E","F","G","H","I","J","K","L",
              "M","N","O","P","Q","R","S","T",
              "U","V","W","X","Y","Z"
            };

            //对传入网址进行MD5加密
            string hex = MD5Util.Get(url);
            string[] strings = new string[4];
            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加
                    outChars += chars[index];
                    //每次循环按位右移5位
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组
                strings[i] = outChars;
            }
            return strings;
        }
    }

    /// <summary>
    /// A StringTokenizer java like object 
    /// </summary>
    public class StringTokenizer : IEnumerable<string>
    {
        private const string _defaultDelim = " \t\n\r\f";
        private string _origin;
        private string _delim;
        private bool _returnDelim;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public StringTokenizer(string str)
        {
            _origin = str;
            _delim = _defaultDelim;
            _returnDelim = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delim"></param>
        public StringTokenizer(string str, string delim)
        {
            _origin = str;
            _delim = delim;
            _returnDelim = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delim"></param>
        /// <param name="returnDelims"></param>
        public StringTokenizer(string str, string delim, bool returnDelims)
        {
            _origin = str;
            _delim = delim;
            _returnDelim = returnDelims;
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return new StringTokenizerEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new StringTokenizerEnumerator(this);
        }

        #endregion


        private class StringTokenizerEnumerator : IEnumerator<string>
        {
            private StringTokenizer _stokenizer;
            private int _cursor = 0;
            private String _next = null;

            public StringTokenizerEnumerator(StringTokenizer stok)
            {
                _stokenizer = stok;
            }

            #region IEnumerator<string> Members

            public string Current
            {
                get { return _next; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _next = GetNext();
                return _next != null;
            }

            public void Reset()
            {
                _cursor = 0;
            }

            #endregion

            private string GetNext()
            {
                char c;
                bool isDelim;

                if (_cursor >= _stokenizer._origin.Length)
                    return null;

                c = _stokenizer._origin[_cursor];
                isDelim = (_stokenizer._delim.IndexOf(c) != -1);

                if (isDelim)
                {
                    _cursor++;
                    if (_stokenizer._returnDelim)
                    {
                        return c.ToString();
                    }
                    return GetNext();
                }

                int nextDelimPos = _stokenizer._origin.IndexOfAny(_stokenizer._delim.ToCharArray(), _cursor);
                if (nextDelimPos == -1)
                {
                    nextDelimPos = _stokenizer._origin.Length;
                }

                string nextToken = _stokenizer._origin.Substring(_cursor, nextDelimPos - _cursor);
                _cursor = nextDelimPos;
                return nextToken;
            }
        }
    }

    public class TypeNameParser
    {
        private readonly string defaultNamespace;
        private readonly string defaultAssembly;
        private static readonly Regex WhiteSpaces = new Regex(@"[\t\r\n]", RegexOptions.Compiled);
        private static readonly Regex MultipleSpaces = new Regex(@"[ ]+", RegexOptions.Compiled);

        public TypeNameParser(string defaultNamespace, string defaultAssembly)
        {
            this.defaultNamespace = defaultNamespace;
            this.defaultAssembly = defaultAssembly;
        }

        public static AssemblyQualifiedTypeName Parse(string type)
        {
            return Parse(type, null, null);
        }

        public static AssemblyQualifiedTypeName Parse(string type, string defaultNamespace, string defaultAssembly)
        {
            return new TypeNameParser(defaultNamespace, defaultAssembly).ParseTypeName(type);
        }

        public AssemblyQualifiedTypeName ParseTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            var type = WhiteSpaces.Replace(typeName, " ");
            type = MultipleSpaces.Replace(type, " ").Replace(", [", ",[").Replace("[ [", "[[").Replace("] ]", "]]");
            if (type.Trim(' ', '[', ']', '\\', ',') == string.Empty)
            {
                throw new ArgumentException(string.Format("The type to parse is not a type name:{0}", typeName), "typeName");
            }
            int genericTypeArgsStartIdx = type.IndexOf('[');
            int genericTypeArgsEndIdx = type.LastIndexOf(']');
            int genericTypeCardinalityIdx = -1;
            if (genericTypeArgsStartIdx >= 0)
            {
                genericTypeCardinalityIdx = type.IndexOf('`', 0, genericTypeArgsStartIdx);
            }

            if (genericTypeArgsStartIdx == -1 || genericTypeCardinalityIdx == -1)
            {
                return ParseNonGenericType(type);
            }
            else
            {
                var isArrayType = type.EndsWith("[]");
                if (genericTypeCardinalityIdx < 0)
                {
                    throw new Exception("Invalid generic fully-qualified type name:" + type);
                }
                // the follow will fail with a generic class with more the 9 type-args (I would see that entity class)
                string cardinalityString = type.Substring(genericTypeCardinalityIdx + 1, 1);
                int genericTypeCardinality = int.Parse(cardinalityString);

                // get the FullName of the non-generic type
                string fullName = type.Substring(0, genericTypeArgsStartIdx);
                if (type.Length - genericTypeArgsEndIdx - 1 > 0)
                    fullName += type.Substring(genericTypeArgsEndIdx + 1, type.Length - genericTypeArgsEndIdx - 1);

                // parse the type arguments
                var genericTypeArgs = new List<AssemblyQualifiedTypeName>();
                string typeArguments = type.Substring(genericTypeArgsStartIdx + 1, genericTypeArgsEndIdx - genericTypeArgsStartIdx - 1);
                foreach (string item in GenericTypesArguments(typeArguments, genericTypeCardinality))
                {
                    var typeArgument = ParseTypeName(item);
                    genericTypeArgs.Add(typeArgument);
                }

                // construct the generic type definition
                return MakeGenericType(ParseNonGenericType(fullName), isArrayType, genericTypeArgs.ToArray());
            }
        }

        public AssemblyQualifiedTypeName MakeGenericType(AssemblyQualifiedTypeName qualifiedName, bool isArrayType,
                                                         AssemblyQualifiedTypeName[] typeArguments)
        {
            Debug.Assert(typeArguments.Length > 0);

            var baseType = qualifiedName.Type;
            var sb = new StringBuilder(typeArguments.Length * 200);
            sb.Append(baseType);
            sb.Append('[');
            for (int i = 0; i < typeArguments.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                sb.Append('[').Append(typeArguments[i].ToString()).Append(']');
            }
            sb.Append(']');
            if (isArrayType)
            {
                sb.Append("[]");
            }
            return new AssemblyQualifiedTypeName(sb.ToString(), qualifiedName.Assembly);
        }

        private static IEnumerable<string> GenericTypesArguments(string s, int cardinality)
        {
            Debug.Assert(cardinality != 0);
            Debug.Assert(string.IsNullOrEmpty(s) == false);
            Debug.Assert(s.Length > 0);

            int startIndex = 0;
            while (cardinality > 0)
            {
                var sb = new StringBuilder(s.Length);
                int bracketCount = 0;

                for (int i = startIndex; i < s.Length; i++)
                {
                    switch (s[i])
                    {
                        case '[':
                            bracketCount++;
                            continue;

                        case ']':
                            if (--bracketCount == 0)
                            {
                                string item = s.Substring(startIndex + 1, i - startIndex - 1);
                                yield return item;
                                sb = new StringBuilder(s.Length);
                                startIndex = i + 2; // 2 = '[' + ']'
                            }
                            break;

                        default:
                            sb.Append(s[i]);
                            continue;
                    }
                }

                if (bracketCount != 0)
                {
                    throw new Exception(string.Format("The brackets are unbalanced in the type name: {0}", s));
                }
                if (sb.Length > 0)
                {
                    var result = sb.ToString();
                    startIndex += result.Length;
                    yield return result.TrimStart(' ', ',');
                }
                cardinality--;
            }
        }

        private AssemblyQualifiedTypeName ParseNonGenericType(string typeName)
        {
            string typeFullName;
            string assembliQualifiedName;

            if (NeedDefaultAssembly(typeName))
            {
                assembliQualifiedName = defaultAssembly;
                typeFullName = typeName;
            }
            else
            {
                int assemblyFullNameIdx = FindAssemblyQualifiedNameStartIndex(typeName);
                if (assemblyFullNameIdx > 0)
                {
                    assembliQualifiedName =
                        typeName.Substring(assemblyFullNameIdx + 1, typeName.Length - assemblyFullNameIdx - 1).Trim();
                    typeFullName = typeName.Substring(0, assemblyFullNameIdx).Trim();
                }
                else
                {
                    assembliQualifiedName = null;
                    typeFullName = typeName.Trim();
                }
            }

            if (NeedDefaultNamespace(typeFullName) && !string.IsNullOrEmpty(defaultNamespace))
            {
                typeFullName = string.Concat(defaultNamespace, ".", typeFullName);
            }

            return new AssemblyQualifiedTypeName(typeFullName, assembliQualifiedName);
        }

        private static int FindAssemblyQualifiedNameStartIndex(string typeName)
        {
            for (int i = 0; i < typeName.Length; i++)
            {
                if (typeName[i] == ',' && typeName[i - 1] != '\\')
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool NeedDefaultNamespaceOrDefaultAssembly(string typeFullName)
        {
            return !typeFullName.StartsWith("System."); // ugly
        }

        private static bool NeedDefaultNamespace(string typeFullName)
        {
            if (!NeedDefaultNamespaceOrDefaultAssembly(typeFullName))
            {
                return false;
            }
            int assemblyFullNameIndex = typeFullName.IndexOf(',');
            int firstDotIndex = typeFullName.IndexOf('.');
            // does not have a dot or the dot is part of AssemblyFullName
            return firstDotIndex < 0 || (firstDotIndex > assemblyFullNameIndex && assemblyFullNameIndex > 0);
        }

        private static bool NeedDefaultAssembly(string typeFullName)
        {
            return NeedDefaultNamespaceOrDefaultAssembly(typeFullName) && typeFullName.IndexOf(',') < 0;
        }
    }

    public class AssemblyQualifiedTypeName
    {
        private readonly string type;
        private readonly string assembly;
        private readonly int hashCode;

        public AssemblyQualifiedTypeName(string type, string assembly)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.type = type;
            this.assembly = assembly;
            unchecked
            {
                hashCode = (type.GetHashCode() * 397) ^ (assembly != null ? assembly.GetHashCode() : 0);
            }
        }

        public string Type
        {
            get { return type; }
        }

        public string Assembly
        {
            get { return assembly; }
        }

        public override bool Equals(object obj)
        {
            AssemblyQualifiedTypeName other = obj as AssemblyQualifiedTypeName;
            return Equals(other);
        }

        public override string ToString()
        {
            if (assembly == null)
            {
                return type;
            }

            return string.Concat(type, ", ", assembly);
        }

        public bool Equals(AssemblyQualifiedTypeName obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return Equals(obj.type, type) && Equals(obj.assembly, assembly);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
