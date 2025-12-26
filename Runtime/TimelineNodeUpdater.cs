using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    // 这个跟ParallelGroup不是一个含义, 应该是像执行器一样的东西
    public class TimelineNodeUpdater: IUpdateNode
    {
        public List<TimelineNode> _runningNodes = new List<TimelineNode>();

        public bool Continue => false;
        public bool Complete => _runningNodes.Count == 0;

        public void Update(float delta, float rate)
        {
            for (int i = _runningNodes.Count - 1; i >= 0; i--)
            {
                TimelineNode current = _runningNodes[i];
                current.updateNode.Update(delta, rate);
                if (current.updateNode.Complete)
                {
                    _runningNodes.RemoveAt(i);
                    if (current._next.Count > 0)
                    {
                        _runningNodes.AddRange(current._next);
                    }
                }
            }
        }

        public void Add(TimelineNode node)
        {
            if (node == null)
                return;

            _runningNodes.Add(node);
        }

        public static TimelineNodeUpdater Get() => new TimelineNodeUpdater();

        public static TimelineNodeUpdater Get(TimelineNode node)
        {
            TimelineNodeUpdater updater = Get();
            updater.Add(node);
            return updater;
        }
    }
}
