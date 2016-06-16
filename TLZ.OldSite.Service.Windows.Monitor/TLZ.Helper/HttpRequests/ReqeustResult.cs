using System.Collections.Generic;

namespace TLZ.Helper.HttpRequests
{
    public class ReqeustResult
    {
        private List<Header> headers = new List<Header>();

        public int StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// 头部信息
        /// </summary>
        public List<Header> Headers
        {
            get
            {
                return headers;
            }
        }

        /// <summary>
        /// Html值
        /// </summary>
        public string Html
        {
            get;
            set;
        }

        public string GetHeader(string key)
        {
            foreach (Header header in Headers)
            {
                if (header.Key == key)
                {
                    return header.Value;
                }
            }
            return "";
        }
    }
}
