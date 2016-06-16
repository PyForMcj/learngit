using System;
using System.Collections.Generic;
using System.Linq;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 并行缓存上下文
    /// </summary>
    public interface IParallelCacheContext
    {
        ITask<T> CreateContextAwareTask<T>(Func<T> function);

        IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector);
    }

    public interface ITask<T> : IDisposable
    {
        T Execute();

        IEnumerable<IVolatileToken> Tokens { get; }

        void Finish();
    }

    public class DefaultParallelCacheContext : IParallelCacheContext
    {
        private readonly ICacheContextAccessor _cacheContextAccessor;

        public DefaultParallelCacheContext(ICacheContextAccessor cacheContextAccessor)
        {
            _cacheContextAccessor = cacheContextAccessor;
        }

        public bool Disabled { get; set; }

        public IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector)
        {
            if (Disabled)
            {
                return source.Select(selector);
            }
            else
            {
                var tasks = source.Select(item => this.CreateContextAwareTask(() => selector(item))).ToList();

                var result = tasks
                    .AsParallel()
                    .AsOrdered()
                    .Select(task => task.Execute())
                    .ToArray();

                foreach (var task in tasks)
                {
                    task.Finish();
                }
                return result;
            }
        }

        public ITask<T> CreateContextAwareTask<T>(Func<T> function)
        {
            return new TaskWithAcquireContext<T>(_cacheContextAccessor, function);
        }

        public class TaskWithAcquireContext<T> : ITask<T>
        {
            private readonly ICacheContextAccessor _cacheContextAccessor;
            private readonly Func<T> _function;
            private IList<IVolatileToken> _tokens;

            public TaskWithAcquireContext(ICacheContextAccessor cacheContextAccessor, Func<T> function)
            {
                _cacheContextAccessor = cacheContextAccessor;
                _function = function;
            }

            public T Execute()
            {
                IAcquireContext parentContext = _cacheContextAccessor.Current;
                try
                {
                    if (parentContext == null)
                    {
                        _cacheContextAccessor.Current = new SimpleAcquireContext(AddToken);
                    }

                    return _function();
                }
                finally
                {
                    if (parentContext == null)
                    {
                        _cacheContextAccessor.Current = parentContext;
                    }
                }
            }

            public IEnumerable<IVolatileToken> Tokens
            {
                get
                {
                    if (_tokens == null)
                        return Enumerable.Empty<IVolatileToken>();
                    return _tokens;
                }
            }

            public void Dispose()
            {
                Finish();
            }

            public void Finish()
            {
                var tokens = _tokens;
                _tokens = null;
                if (_cacheContextAccessor.Current != null && tokens != null)
                {
                    foreach (var token in tokens)
                    {
                        _cacheContextAccessor.Current.Monitor(token);
                    }
                }
            }

            private void AddToken(IVolatileToken token)
            {
                if (_tokens == null)
                    _tokens = new List<IVolatileToken>();
                _tokens.Add(token);
            }
        }
    }
}
