using System;
using System.Collections.Concurrent;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// Cache Holder
    /// </summary>
    public interface ICacheHolder : ISingletonDependency
    {
        ICache<TKey, TResult> GetCache<TKey, TResult>();
    }

    public class DefaultCacheHolder : ICacheHolder
    {
        private readonly ICacheContextAccessor _cacheContextAccessor;
        private readonly ConcurrentDictionary<CacheKey, object> _caches = new ConcurrentDictionary<CacheKey, object>();

        public DefaultCacheHolder(ICacheContextAccessor cacheContextAccessor)
        {
            _cacheContextAccessor = cacheContextAccessor;
        }

        class CacheKey : Tuple< Type, Type>
        {
            public CacheKey( Type key, Type result)
                : base( key, result)
            {
            }
        }

        public ICache<TKey, TResult> GetCache<TKey, TResult>()
        {
            var cacheKey = new CacheKey( typeof(TKey), typeof(TResult));
            var result = _caches.GetOrAdd(cacheKey, k => new Cache<TKey, TResult>(_cacheContextAccessor));
            return (Cache<TKey, TResult>)result;
        }
    }
}
