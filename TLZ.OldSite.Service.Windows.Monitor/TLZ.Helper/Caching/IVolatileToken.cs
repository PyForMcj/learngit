namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 缓存监控令牌
    /// </summary>
    public interface IVolatileToken
    {
        /// <summary>
        /// 是否已经到达该令牌了，即缓存是否应该失效了.true=未失效，false=已失效
        /// </summary>
        bool IsCurrent { get; }
    }
}
