using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Timeline
{
    public static class EnumeratorUtils
    {
        public static WhenAllEnumerator WhenAll(params IEnumerator[] enumerators)
        {
            return WhenAll(enumerators as IEnumerable<IEnumerator>);
        }

        public static WhenAllEnumerator WhenAll(IEnumerable<IEnumerator> enumerators)
        {
            return new WhenAllEnumerator(enumerators);
        }
    }

    public struct WhenAllEnumerator: IEnumerable, IEnumerator
    {
        private IEnumerator[] _enumerators;
        private int _count;

        public WhenAllEnumerator(IEnumerable<IEnumerator> enumerators)
        {
            _enumerators = enumerators.ToArray();
            _count = _enumerators.Length;
        }

        public bool MoveNext()
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                var current = _enumerators[i];
                if (current == null)
                    continue;

                bool next = current.MoveNext();
                if (!next)
                {
                    _enumerators[i] = null;
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

        public object Current
        {
            get
            {
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public WhenAllEnumerator GetEnumerator()
        {
            return this;
        }
    }
}
