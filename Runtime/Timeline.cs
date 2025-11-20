using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // 加一个按钮可以用于Debug目前在Track里面的Timeline?
    // 用List确实是比较好的, 维护一个index
    // 然后就是自己实现一个双端队列
    [Serializable]
    public class Timeline: ParallelNode
    {
        // 为什么是LinkedList? 有这个必要吗? 因为是从前往后? 先进先出? 为什么不适用Queue呢?
        private LinkedList<ParallelNode> _orderNodes = new LinkedList<ParallelNode>();
        // private Queue<ParallelNode> orderNodes = new Queue<ParallelNode>();

        public float _rate = 1;

        public string _name = string.Empty;

        public override bool Update(float delta, float rate)
        {
            bool parallelDone = UpdateParallel(delta * _rate, rate * _rate);
            bool orderDone = UpdateOrder(delta * _rate, rate * _rate);
            return parallelDone && orderDone;
        }

        public bool UpdateOrder(float delta, float rate)
        {
            LinkedListNode<ParallelNode> node = _orderNodes.First;
            // orderNodes.TryPeek() // 如果是队列的话, Peek的是哪边? 队列如果对比List, index小的那边是首还是大的那边是首? Peek的是最早加入的还是最晚加入的
            // 队列适合访问到最早加入的元素, 栈适合访问到最晚加入的元素

            if (node == null)
                return true;

            ParallelNode value =node.Value;

            if (value.Update(_rate * delta, _rate * rate))
            {
                _orderNodes.RemoveFirst();
            }

            return false;
        }

        public IEnumerable<TimelineNode> FindNodes(string tag)
        {
            foreach (var parallelNode in _orderNodes)
            {
                if (parallelNode is NodeGroup nodeGroup)
                {
                    foreach (var node in nodeGroup.FindNodes(tag))
                        yield return node;
                }
                else if (parallelNode is Timeline timeline)
                {
                    foreach (var node in timeline.FindNodes(tag))
                        yield return node;
                }
            }
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

        // 为什么没有显示t2?
        public override string ToString(int level)
        {
            string output = new string('\t', level) + "timeline " + $"{_name}" + '\n';
            level++;
            foreach (var orderNode in _orderNodes)
            {
                output += orderNode.ToString(level) + '\n';
            }
            return output;
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

        public void Chain(TimelineNode node)
        {
            if (node != null)
            {
                NodeGroup group = NodeGroup.Get();
                group.Parallel(node);
                _orderNodes.AddLast(group);
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
                _orderNodes.AddLast(timeline);
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
                _orderNodes.AddLast(node);
            }
        }

        public void Group(TimelineNode node)
        {
            if (node != null)
            {
                // 为什么是Last? Join是编排方法, 只对Last生效
                if (_orderNodes.Last != null)
                {
                    _orderNodes.Last.Value.Parallel(node);
                }
                else
                {
                    Chain(node);
                }
            }
        }

        public void Group(IEnumerable<TimelineNode> nodes)
        {
            if (nodes == null)
                return;

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
                if (_orderNodes.Last != null)
                {
                    _orderNodes.Last.Value.Parallel(node);
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

        public static Timeline Get(string name)
        {
            var t = new Timeline
            {
                _name = name
            };
            return t;
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
                    result.Chain(timeline);
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
