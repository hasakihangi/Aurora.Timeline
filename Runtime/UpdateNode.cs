using System;

namespace Aurora.Timeline
{
    public struct UpdateNode: IUpdater
    {
        public UpdateMethod _method;
        public Action _onDone;
        public float _elapsed;
        public float _rate;

        public UpdateNode(UpdateMethod method): this()
        {
            _method = method;
            _onDone = null;
            _rate = 1;
            _elapsed = 0;
        }

        public UpdateNode(Action onDone): this()
        {
            _method = null;
            _onDone = onDone;
            _rate = 1;
            _elapsed = 0;
        }

        public static UpdateNode Action(Action action) => new UpdateNode((delta, elapsed, rate) =>
        {
            action?.Invoke();
            return true;
        });

        public static UpdateNode Done(Action onDone) => new UpdateNode(onDone);

        public static UpdateNode Delay(float seconds) => new UpdateNode((delta, elapsed, rate) =>
        {
            return elapsed >= seconds;
        });

        public static UpdateNode Delay(float seconds, Action onDone)
        {
            UpdateNode node = Delay(seconds);
            node._onDone = onDone;
            return node;
        }

        public float Rate => _rate;
        public bool Update(float delta, float rate)
        {
            delta = delta * _rate;
            bool done = _method == null || _method.Invoke(delta, _elapsed, rate * _rate);
            _elapsed += delta;

            if (done)
            {
                _onDone?.Invoke();
                _onDone = null;
            }

            return done;
        }
    }

    public delegate bool UpdateMethod(float delta, float elapsed, float rate);
}
