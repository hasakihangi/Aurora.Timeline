using System;
using System.Collections;

namespace Aurora.Timeline
{
    public class EnumeratorUpdater: IUpdateNode
    {
        private IEnumerator _enumerator;

        public EnumeratorUpdater(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public void Update(float delta, float rate)
        {
            bool next = _enumerator.MoveNext();
            Completed = !next;
        }

        public bool Completed { get; private set; }
        public bool Finished => Completed;
    }
}
