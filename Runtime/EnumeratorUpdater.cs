using System;
using System.Collections;

namespace Aurora.Timeline
{
    public class EnumeratorUpdater: IUpdater
    {
        public IEnumerator _enumerator;

        public EnumeratorUpdater(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public bool Update(float delta, float rate)
        {
            return !_enumerator.MoveNext();
        }
    }
}
