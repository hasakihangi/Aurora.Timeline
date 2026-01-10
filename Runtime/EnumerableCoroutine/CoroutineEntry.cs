
using System;
using System.Collections;

namespace Aurora.Timeline.EnumerableCoroutine
{
    public struct CoroutineEntry: IDisposable
    {
        public CoroutineHandle handle;
        public IEnumerator enumerator;

        public CoroutineEntry(CoroutineHandle handle, IEnumerator enumerator)
        {
            this.handle = handle;
            this.enumerator = enumerator;
        }

        public void Dispose()
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    public class CoroutineHandle
    {
        private bool cancel = false;
        public bool Cancel => cancel;

        public void SetCancel()
        {
            cancel = true;
        }
    }
}
