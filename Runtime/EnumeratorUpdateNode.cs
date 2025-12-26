using System;
using System.Collections;

namespace Aurora.Timeline
{
    public class EnumeratorUpdateNode: IUpdateNode
    {
        public IEnumerator _enumerator;

        public EnumeratorUpdateNode(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public void Update(float delta, float rate)
        {
            bool next = _enumerator.MoveNext();
            Complete = !next;
        }

        public bool Complete { get; private set; }
        public bool Continue => false;
    }
}
