using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TLZ.Helper.HttpRequests
{
    public class RequestData
    {
        private readonly List<FormValue> _formValue = new List<FormValue>();
        private readonly List<Header> _headers = new List<Header>();
        private RequestMethods _method = RequestMethods.Post;
        private Encoding _requestEncoding = Encoding.GetEncoding("UTF-8");
        private Encoding _responseEncoding = Encoding.GetEncoding("UTF-8");
        private int _timeout = 300000;
        private string _contentType = "application/x-www-form-urlencoded";
        private string _userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
        private string _accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
        private bool _keepAlive = true;

        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestMethods Method
        {
            get
            {
                return _method;
            }
            set
            {
                _method = value;
            }

        }

        /// <summary>
        /// 请求的编码格式，默认为UTF8
        /// </summary>
        public Encoding RequestEncoding
        {
            get
            {
                return _requestEncoding;
            }
            set
            {
                _requestEncoding = value;
            }
        }

        /// <summary>
        /// 响应的编码格式，默认为UTF8
        /// </summary>
        public Encoding ResponseEncoding
        {
            get
            {
                return _responseEncoding;
            }
            set
            {
                _responseEncoding = value;
            }
        }

        /// <summary>
        /// 响应时间
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        /// <summary>
        /// 响应类容类型
        /// </summary>
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        /// <summary>
        /// 代理值
        /// </summary>
        public string UserAgent
        {
            get
            {
                return _userAgent;
            }
            set
            {
                _userAgent = value;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string Accept
        {
            get
            {
                return _accept;
            }
            set
            {
                _accept = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool KeepAlive
        {
            get
            {
                return _keepAlive;
            }
            set
            {
                _keepAlive = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WebProxy WebProxy
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
                return _headers;
            }
        }

        /// <summary>
        /// Form提交的表单内容。
        /// </summary>
        public List<FormValue> FormValue
        {
            get
            {
                return _formValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddFormValue(string name, string value)
        {
            FormValue item = new FormValue
            {
                Name = name,
                Value = value
            };
            FormValue.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
        {
            Header item = new Header
            {
                Key = key,
                Value = value
            };
            Headers.Add(item);
        }


    }
}
