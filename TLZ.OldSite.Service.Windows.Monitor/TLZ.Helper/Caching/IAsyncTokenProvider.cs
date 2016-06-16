using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 异步令牌提供器
    /// </summary>
    public interface IAsyncTokenProvider
    {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }

    public class DefaultAsyncTokenProvider : IAsyncTokenProvider
    {
        public DefaultAsyncTokenProvider()
        {
            //Logger = NullLogger.Instance;
        }

        //public ILogger Logger { get; set; }

        public IVolatileToken GetToken(Action<Action<IVolatileToken>> task)
        {
            var token = new AsyncVolativeToken(task/*, Logger*/);
            token.QueueWorkItem();
            return token;
        }

        public class AsyncVolativeToken : IVolatileToken
        {
            private readonly Action<Action<IVolatileToken>> _task;
            private readonly List<IVolatileToken> _taskTokens = new List<IVolatileToken>();
            private volatile Exception _taskException;
            private volatile bool _isTaskFinished;

            public AsyncVolativeToken(Action<Action<IVolatileToken>> task/*, ILogger logger*/)
            {
                _task = task;
                //Logger = logger;
            }

            //public ILogger Logger { get; set; }

            public void QueueWorkItem()
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        _task(token => _taskTokens.Add(token));
                    }
                    catch (Exception e)
                    {
                        //Logger.Error(e, "监控扩展出错.");
                        _taskException = e;
                    }
                    finally
                    {
                        _isTaskFinished = true;
                    }
                });
            }

            public bool IsCurrent
            {
                get
                {
                    // 出错了 缓存失效
                    if (_taskException != null)
                    {
                        return false;
                    }
                    if (_isTaskFinished)
                    {
                        return _taskTokens.All(t => t.IsCurrent);
                    }
                    //任务为完成 默认不失效
                    return true;
                }
            }
        }
    }

}
