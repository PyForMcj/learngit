using System;

namespace TLZ.Helper.Caching
{
    //表示弱引用，即在引用对象的同时仍然允许垃圾回收来回收该对象。
    public class Weak<T>
    {
        private readonly WeakReference _target;

        public Weak(T target)
        {
            _target = new WeakReference(target);
        }

        public Weak(T target, bool trackResurrection)
        {
            _target = new WeakReference(target, trackResurrection);
        }

        public T Target
        {
            get { return (T)_target.Target; }
            set { _target.Target = value; }
        }
    }
}
