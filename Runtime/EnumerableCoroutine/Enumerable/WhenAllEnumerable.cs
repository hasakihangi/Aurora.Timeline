using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Timeline.EnumerableCoroutine
{
    public struct WhenAllEnumerable: IEnumerable, IEnumerator
    {
        private IEnumerator[] _enumerators;
        private int _count;

        public WhenAllEnumerable(params IEnumerable[] enumerables)
        {
            _enumerators = enumerables.Select(enumerable => enumerable.GetEnumerator()).ToArray();
            _count = _enumerators.Length;
        }

        public bool MoveNext()
        {
            for (int i = 0; i < _enumerators.Length; i++)
            {
                var enumerator = _enumerators[i];
                if (enumerator == null)
                    continue;

                bool next = enumerator.MoveNext();
                if (!next)
                {
                    _enumerators[i] = null;
                    (enumerator as IDisposable)?.Dispose();
                    _count --;
                }
            }

            if (_count == 0)
                return false;

            return true;
        }

        public void Reset()
        {

        }

        public object Current => null;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public WhenAllEnumerable GetEnumerator()
        {
            return this;
        }
    }
}
