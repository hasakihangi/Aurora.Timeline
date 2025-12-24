using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    // 这个跟ParallelGroup不是一个含义, 应该是像执行器一样的东西
    public class TimelineNodeUpdater: IUpdater
    {
        public List<TimelineNode> _runningNodes = new List<TimelineNode>();

        public bool Update(float delta, float rate)
        {
            for (int i = _runningNodes.Count - 1; i >= 0; i--)
            {
                TimelineNode current = _runningNodes[i];

                if (current.updateNode.Update(delta, rate))
                {
                    // 如果是SetTimelineNode的Cancel会递归传递
                    _runningNodes.RemoveAt(i);
                    if (current._next.Count > 0)
                    {
                        _runningNodes.AddRange(current._next);
                    }
                }
            }

            if (_runningNodes.Count == 0)
                return true;

            return false;
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
