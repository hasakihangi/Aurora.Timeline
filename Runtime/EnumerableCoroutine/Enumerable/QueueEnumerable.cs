using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Timeline.EnumerableCoroutine
{
    public struct QueueEnumerable : IEnumerable, IEnumerator
    {
        private IEnumerator[] _enumerators;
        private int _index;

        public QueueEnumerable(params IEnumerable[] enumerables)
        {
            _enumerators = enumerables.Select(enumerable => enumerable.GetEnumerator()).ToArray();
            _index = 0;
        }

        public bool MoveNext()
        {
            if (_index >= _enumerators.Length)
                return false;

            var currentEnumerator = _enumerators[_index];
            bool hasNext = currentEnumerator.MoveNext();

            if (!hasNext)
            {
                (currentEnumerator as IDisposable)?.Dispose();
                _enumerators[_index] = null;

                _index++;

                if (_index >= _enumerators.Length)
                    return false;
            }

            return true;
        }

        public void Reset()
        {

        }

        public object Current => null;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public QueueEnumerable GetEnumerator()
        {
            return this;
        }
    }
}
