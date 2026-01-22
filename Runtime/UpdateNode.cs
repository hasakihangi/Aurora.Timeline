using System;

namespace Aurora.Timeline
{
    public struct UpdateNode: IUpdateNode
    {
        public UpdateMethod method;
        public Action onCompleted;
        public float rate;

        private float _elapsed;
        public UpdateNode(UpdateMethod method): this()
        {
            this.method = method;
            onCompleted = null;
            rate = 1;
            _elapsed = 0;
        }

        public UpdateNode(Action onDone): this()
        {
            method = null;
            this.onCompleted = onDone;
            rate = 1;
            _elapsed = 0;
        }

        public static UpdateNode ActionNode(Action action) => new UpdateNode((delta, elapsed, rate) =>
        {
            action?.Invoke();
            return true;
        });

        public static UpdateNode DoneNode(Action onDone) => new UpdateNode(onDone);

        public static UpdateNode DelayNode(float seconds) => new UpdateNode((delta, elapsed, rate) =>
        {
            return elapsed >= seconds;
        });

        public static UpdateNode Delay(float seconds, Action onDone)
        {
            UpdateNode node = DelayNode(seconds);
            node.onCompleted = onDone;
            return node;
        }

        public void Update(float delta, float rate)
        {
            delta = delta * this.rate;
            rate = rate * this.rate;
            bool done = method == null || method.Invoke(delta, _elapsed, rate);
            _elapsed += delta;

            if (done)
            {
                onCompleted?.Invoke();
                onCompleted = null;
                Completed = true;
            }
        }

        public bool Finished => Completed;
        public bool Completed {get; private set;}
    }

    public delegate bool UpdateMethod(float delta, float elapsed, float rate);
}
