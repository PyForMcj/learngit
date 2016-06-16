using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLZ.MongoDB
{
    public class MongoDBCache<TKey, TObject> : IDisposable
    {
        /// <summary>
        /// 缓存对象的容器
        /// </summary>
        private ConcurrentDictionary<TKey, TObject> _Dictionary = null;
        public MongoDBCache()
        {
            _Dictionary = new ConcurrentDictionary<TKey, TObject>();
             MongoDBHelper.ConfigChangedEvent += Clear;
        }
        ~MongoDBCache()
        {
            MongoDBHelper.ConfigChangedEvent -= Clear;
        }
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            _Dictionary.Clear();
        }
        /// <summary>
        /// 获取缓存中的对象
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>缓存数据</returns>
        public TObject GetTObject(TKey key)
        {
            TObject tObject = default(TObject);
            _Dictionary.TryGetValue(key, out tObject);
            return tObject;
        }
        /// <summary>
        /// 进入缓存对象
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="newTObject">要缓存数据</param>
        /// <returns>True表示进入缓存，False表示没有进入缓存</returns>
        public bool AddOrUpdateTObject(TKey key, TObject newTObject)
        {
            bool result = false;
            TObject oldTObject = default(TObject);
            if (_Dictionary.TryGetValue(key, out oldTObject))
            {
                result = _Dictionary.TryUpdate(key, newTObject, oldTObject);
            }
            else
            {
                result = _Dictionary.TryAdd(key, newTObject);
            }
            return result;
        }
        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="key">主键</param>
        public void DeleteTObject(TKey key)
        {
            TObject tObject = default(TObject);
            _Dictionary.TryRemove(key, out tObject);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
