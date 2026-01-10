using System;
using System.Collections;

namespace Aurora.Timeline.EnumerableCoroutine
{
    public struct DelayEnumerable : IEnumerable, IEnumerator
    {
        private readonly float _duration;
        private float _elapsed;

        public DelayEnumerable(float seconds)
        {
            _duration = seconds;
            _elapsed = 0f;
        }

        public bool MoveNext()
        {
            _elapsed += UnityEngine.Time.deltaTime;
            return _elapsed < _duration;
        }

        public void Reset()
        {
            _elapsed = 0f;
        }

        public object Current => null;

        public DelayEnumerable GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}

