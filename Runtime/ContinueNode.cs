using System;

namespace Aurora.Timeline
{
    public struct ContinueNode : IUpdateNode
    {
        public ContinueMethod continueMethod;
        // public Action onCompleted; // 这个真的有必要吗? 或者是下一个节点?
        public float rate;

        private float _elapsed;

        public ContinueNode(ContinueMethod method): this()
        {
            this.continueMethod = method;
            // onCompleted = null;
            rate = 1;
            _elapsed = 0;
        }

        public void Update(float delta, float rate)
        {
            delta = delta * this.rate;
            rate = rate * this.rate;

            bool completed = false;
            bool ceased = false;

            if (continueMethod == null)
            {
                completed = true;
                ceased = true;
            }
            else
            {
                continueMethod.Invoke(delta, _elapsed, rate, out completed, out ceased);
            }

            _elapsed += delta;

            Completed = completed;
            _ceased = ceased;
        }

        public bool Completed { get; private set;}
        private bool _ceased;
        public bool Finished => Completed && _ceased;
    }

    public delegate void ContinueMethod(float delta, float elapsed, float rate, out bool complete, out bool cease);
}
