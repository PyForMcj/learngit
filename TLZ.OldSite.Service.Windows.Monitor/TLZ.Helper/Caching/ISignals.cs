using System.Collections.Generic;

namespace TLZ.Helper.Caching
{
    /// <summary>
    /// 信号量监视器
    /// </summary>
    public interface ISignals : IVolatileProvider
    {
        /// <summary>
        /// 触发指定的信息量失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signal"></param>
        void Trigger<T>(T signal);
        IVolatileToken When<T>(T signal);
    }

    public class Signals : ISignals
    {
        readonly IDictionary<object, Token> _tokens = new Dictionary<object, Token>();

        public void Trigger<T>(T signal)
        {
            lock (_tokens)
            {
                Token token;
                if (_tokens.TryGetValue(signal, out token))
                {
                    _tokens.Remove(signal);
                    token.Trigger();
                }
            }

        }

        public IVolatileToken When<T>(T signal)
        {
            lock (_tokens)
            {
                Token token;
                if (!_tokens.TryGetValue(signal, out token))
                {
                    token = new Token();
                    _tokens[signal] = token;
                }
                return token;
            }
        }

        class Token : IVolatileToken
        {
            public Token()
            {
                IsCurrent = true;
            }
            public bool IsCurrent { get; private set; }
            public void Trigger() { IsCurrent = false; }
        }
    }
}
