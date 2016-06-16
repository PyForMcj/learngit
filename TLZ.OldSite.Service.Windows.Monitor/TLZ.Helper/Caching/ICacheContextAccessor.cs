using System;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 缓存上下文访问器
    /// </summary>
    public interface ICacheContextAccessor
    {
        IAcquireContext Current { get; set; }
    }

    public class DefaultCacheContextAccessor : ICacheContextAccessor
    {
        [ThreadStatic]
        private static IAcquireContext _threadInstance;

        public static IAcquireContext ThreadInstance
        {
            get { return _threadInstance; }
            set { _threadInstance = value; }
        }

        public IAcquireContext Current
        {
            get { return ThreadInstance; }
            set { ThreadInstance = value; }
        }
    }
}
