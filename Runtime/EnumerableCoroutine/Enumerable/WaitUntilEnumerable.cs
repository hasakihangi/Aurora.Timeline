using System;
using System.Collections;

namespace Aurora.Timeline.EnumerableCoroutine
{
    public struct WaitUntilEnumerable : IEnumerable, IEnumerator
    {
        private readonly Func<bool> _predicate;

        public WaitUntilEnumerable(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public bool MoveNext()
        {
            return !_predicate();
        }

        public void Reset() { }

        public object Current => null;

        public WaitUntilEnumerable GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
