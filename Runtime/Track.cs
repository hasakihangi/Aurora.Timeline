using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    [Serializable]
    public class Track
    {
        public List<Timeline> running = new List<Timeline>();
        public List<Timeline> order = new List<Timeline>();

        public float _rate = 1f;

        public void Update(float delta, float rate)
        {
            bool complete = true;
            for (int i = running.Count - 1; i >= 0; i--)
            {
                Timeline current = running[i];
                current.Update(_rate * delta, _rate * rate);

                if (current.Completed)
                {
                    if (!current.Finished)
                    {
                        running.RemoveAt(i);
                    }
                }
                else
                {
                    complete = false;
                }
            }

            if (complete)
            {
                if (order.Count > 0)
                {
                    Run(order[0]);
                    order.RemoveAt(0);
                }
            }
        }

        public void Run(Timeline timeline)
        {
            if (timeline == null)
                return;

            running.Add(timeline);
        }

        public void Append(Timeline timeline)
        {
            if (timeline == null)
                return;

            order.Add(timeline);
        }
    }
}
