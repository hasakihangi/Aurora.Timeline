using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // ParallelNode需要有Join Timeline和TimelineNode的能力
    [Serializable]
    public class Timeline: ParallelNode
    {
        private LinkedList<ParallelNode> orderNodes = new LinkedList<ParallelNode>();

        public float m_Rate = 1;

        public string name = string.Empty;

        public override bool Update(float delta, float rate)
        {
            bool parallelDone = UpdateParallel(delta * m_Rate, rate * m_Rate);
            bool orderDone = UpdateOrder(delta * m_Rate, rate * m_Rate);
            return parallelDone && orderDone;
        }

        public bool UpdateOrder(float delta, float rate)
        {
            LinkedListNode<ParallelNode> node = orderNodes.First;

            if (node == null)
                return true;

            ParallelNode value =node.Value;

            if (value.Update(m_Rate * delta, m_Rate * rate))
            {
                orderNodes.RemoveFirst();
            }

            return false;
        }


        public override void Parallel(TimelineNode node)
        {
            if (node != null)
            {
                NodeGroup group = NodeGroup.Get();
                group.Parallel(node);
                parallelNodes.Add(group);
            }
        }

        public void Parallel(params TimelineNode[] nodes)
        {
            NodeGroup group = null;
            if (nodes.Length > 0)
            {
                foreach (var node in nodes)
                {
                    if (node != null)
                    {
                        if (group == null)
                        {
                            group = NodeGroup.Get();
                        }
                        group.Parallel(node);
                    }
                }
            }

            if (group != null)
            {
                parallelNodes.Add(group);
            }
        }

        // 因为是从first开始执行, 所以加在last上
        public void Chain(TimelineNode node)
        {
            if (node != null)
            {
                NodeGroup group = NodeGroup.Get();
                group.Parallel(node);
                orderNodes.AddLast(group);
            }
        }

        public void Chain(IEnumerable<TimelineNode> nodes)
        {
            foreach (var node in nodes)
            {
                Chain(node);
            }
        }

        public void Chain(params TimelineNode[] nodes)
        {
            Chain(nodes as IEnumerable<TimelineNode>);
        }

        public void Chain(Timeline timeline)
        {
            if (timeline != null)
            {
                orderNodes.AddLast(timeline);
            }
        }

        public void Chain(IEnumerable<Timeline> timelines)
        {
            foreach (var line in timelines)
            {
                Chain(line);
            }
        }

        public void Chain(ParallelNode node)
        {
            if (node != null)
            {
                orderNodes.AddLast(node);
            }
        }

        public void Group(TimelineNode node)
        {
            if (node != null)
            {
                // 为什么是Last? Join是编排方法, 只对Last生效
                if (orderNodes.Last != null)
                {
                    orderNodes.Last.Value.Parallel(node);
                }
                else
                {
                    Chain(node);
                }
            }
        }

        public void Group(IEnumerable<TimelineNode> nodes)
        {
            foreach (var node in nodes)
            {
                Group(node);
            }
        }

        public void Group(params TimelineNode[] nodes)
        {
            Group(nodes as IEnumerable<TimelineNode>);
        }

        public void Group(ParallelNode node)
        {
            if (node != null)
            {
                if (orderNodes.Last != null)
                {
                    orderNodes.Last.Value.Parallel(node);
                }
                else
                {
                    Chain(node);
                }
            }
        }

        public void Group(IEnumerable<Timeline> timelines)
        {
            foreach (var timeline in timelines)
            {
                Group(timeline);
            }
        }

        public static Timeline Nothing => Get();

        private Timeline() {}

        public static Timeline Get()
        {
            return new Timeline();
        }

        public static Timeline Get(TimelineNode node)
        {
            Timeline timeline = Get();
            timeline.Chain(node);
            return timeline;
        }

        public static Timeline ArrangeInOrder(IEnumerable<Timeline> timelines)
        {
            Timeline result = Get();

            if (timelines != null)
            {
                foreach (var timeline in timelines)
                {
                    if (timeline != null)
                    {
                        result.Chain(timeline);
                    }
                }
            }

            return result;
        }

        public static Timeline ArrangeInOrder(params Timeline[] timelines)
        {
            return ArrangeInOrder(timelines as IEnumerable<Timeline>);
        }
    }
}
