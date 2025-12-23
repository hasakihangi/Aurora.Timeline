using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    [Serializable]
    public class Track
    {
        public List<Timeline> running = new List<Timeline>();
        public LinkedList<Timeline> order = new LinkedList<Timeline>();

        public float m_Rate = 1f;

        public void Update(float delta, float rate)
        {
            for (int i = running.Count - 1; i >= 0; i--)
            {
                Timeline current = running[i];
                if (current.Update(m_Rate * delta, m_Rate * rate))
                {
                    running.RemoveAt(i);
                }
            }

            if (running.Count == 0)
            {
                if (order.First != null)
                {
                    Run(order.First.Value);
                    order.RemoveFirst();
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

            order.AddLast(timeline);
        }

        // public void Run(TimelineNode node)
        // {
        //     if (node == null)
        //         return;
        //
        //     Timeline timeline = Timeline.Get(node);
        //     Run(timeline);
        // }
        //
        // public void Append(TimelineNode node)
        // {
        //     if (node == null)
        //         return;
        //
        //     Timeline timeline = Timeline.Get(node);
        //     Append(timeline);
        // }
    }
}
