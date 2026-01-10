using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aurora.Timeline;

namespace Aurora.Timeline.EnumerableCoroutine
{
    internal class CoroutineManager : SingletonBehaviour<CoroutineManager>
    {
        private List<CoroutineEntry> _coroutines = new List<CoroutineEntry>();

        public CoroutineHandle RunCoroutine(IEnumerable enumerable)
        {
            CoroutineHandle handle = new CoroutineHandle();
            IEnumerator enumerator = enumerable.GetEnumerator();
            _coroutines.Add(new CoroutineEntry(handle, enumerator));
            return handle;
        }

        private void Update()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                var current = _coroutines[i];
                if (current.handle.Cancel)
                {
                    _coroutines.RemoveAt(i);
                    current.Dispose();
                    i--;
                    continue;
                }

                bool next = current.enumerator.MoveNext();
                // current.enumerator.Current
                if (!next)
                {
                    _coroutines.RemoveAt(i);
                    current.Dispose();
                    i--;
                }
            }
        }
    }
}
