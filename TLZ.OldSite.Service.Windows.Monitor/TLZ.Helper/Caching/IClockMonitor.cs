using System;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 绝对时间监控器
    /// </summary>
    public interface IAbsoluteClockMonitor : IVolatileProvider
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        DateTime Now { get; }
        /// <summary>
        /// 在当前基础时间上经过指定时间段后过期
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IVolatileToken When(TimeSpan duration);
        /// <summary>
        /// 在当前基础时间上指定时间点后过期
        /// </summary>
        /// <param name="absoluteUtc"></param>
        /// <returns></returns>
        IVolatileToken When(DateTime absolute);
    }
    public class AbsoluteMonitorClock : IAbsoluteClockMonitor
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public IVolatileToken When(TimeSpan duration)
        {
            return new AbsoluteExpirationToken(this, duration);
        }

        public IVolatileToken When(DateTime absolute)
        {
            return new AbsoluteExpirationToken(this, absolute);
        }
        /// <summary>
        /// 过期令牌
        /// </summary>
        public class AbsoluteExpirationToken : IVolatileToken
        {
            private readonly IAbsoluteClockMonitor _clock;
            private readonly DateTime _invalidate;

            public AbsoluteExpirationToken(IAbsoluteClockMonitor clock, DateTime invalidate)
            {
                _clock = clock;
                _invalidate = invalidate;
            }

            public AbsoluteExpirationToken(IAbsoluteClockMonitor clock, TimeSpan duration)
            {
                _clock = clock;
                _invalidate = _clock.Now.Add(duration);
            }

            public bool IsCurrent
            {
                get
                {
                    return _clock.Now < _invalidate;
                }
            }
        }
    }



    /// <summary>
    /// 相对时间监控器
    /// </summary>
    public interface IRelativeClockMonitor : IVolatileProvider
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        DateTime Now { get; }
        /// <summary>
        /// 经过指定时间段后不访问缓存将过期
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IVolatileToken When(TimeSpan duration);
    }
    /// <summary>
    /// 相对监视器
    /// </summary>
    public class RelativeMonitorClock : IRelativeClockMonitor
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public IVolatileToken When(TimeSpan duration)
        {
            return new RelativeExpirationToken(this, duration);
        }
        /// <summary>
        /// 过期令牌
        /// </summary>
        public class RelativeExpirationToken : IVolatileToken
        {
            private readonly IRelativeClockMonitor _clock;
            private readonly TimeSpan _duration;
            private DateTime _invalidate;

            public RelativeExpirationToken(IRelativeClockMonitor clock, TimeSpan duration)
            {
                _clock = clock;
                _duration = duration;
                _invalidate = _clock.Now.Add(duration);
            }

            public bool IsCurrent
            {
                get
                {
                    if (_clock.Now < _invalidate)
                    {
                        _invalidate = _clock.Now.Add(_duration);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
