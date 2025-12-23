using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // 这个只有order可以吗? 如果是parallel
    [Serializable]
    public class Timeline: IUpdateNode
    {
        private Dequeue<ParallelGroup> _orderNodes = new Dequeue<ParallelGroup>();

        public float _rate = 1;
        public string _name = string.Empty;

        // 为什么是这样写? 同时更新Parallel和order?
        public bool Update(float delta, float rate)
        {
            if (Complete)
                return true;

            ParallelGroup group = _orderNodes.PeekFront();

            if (group == null)
                return true;

            if (group.UpdateParallel(_rate * delta, _rate * rate))
            {
                _orderNodes.DequeueFront();
            }

            if (_orderNodes.Count == 0)
                return true;

            return false;
        }


        public bool Complete { private get; set; } = false;


        public void Parallel(IUpdateNode node)
        {
            if (node != null)
            {
                if (_orderNodes.TryPeekFront(out var group))
                {
                    group.Parallel(node);
                }
                else
                {
                    group = ParallelGroup.Get(node);
                }
            }
        }

        // 为什么没有显示t2?
        // public override string ToString(int level)
        // {
        //     string output = new string('\t', level) + "timeline " + $"{_name}" + '\n';
        //     level++;
        //     foreach (var orderNode in _orderNodes)
        //     {
        //         output += orderNode.ToString(level) + '\n';
        //     }
        //     return output;
        // }

        // public void Parallel(params TimelineNode[] nodes)
        // {
        //     TimelineGroupGroup groupGroup = null;
        //     if (nodes.Length > 0)
        //     {
        //         foreach (var node in nodes)
        //         {
        //             if (node != null)
        //             {
        //                 if (groupGroup == null)
        //                 {
        //                     groupGroup = TimelineGroupGroup.Get();
        //                 }
        //                 groupGroup.Parallel(node);
        //             }
        //         }
        //     }
        //
        //     if (groupGroup != null)
        //     {
        //         parallelNodes.Add(groupGroup);
        //     }
        // }

        public void Chain(IUpdateNode node)
        {
            if (node != null)
            {
                ParallelGroup group = ParallelGroup.Get(node);
                _orderNodes.EnqueueBack(group);
            }
        }

        // public void Chain(IEnumerable<TimelineNode> nodes)
        // {
        //     foreach (var node in nodes)
        //     {
        //         Chain(node);
        //     }
        // }

        // public void Chain(params TimelineNode[] nodes)
        // {
        //     Chain(nodes as IEnumerable<TimelineNode>);
        // }

        // public void Chain(Timeline timeline)
        // {
        //     if (timeline != null)
        //     {
        //         _orderNodes.EnqueueBack(timeline);
        //     }
        // }

        // public void Chain(IEnumerable<Timeline> timelines)
        // {
        //     foreach (var line in timelines)
        //     {
        //         Chain(line);
        //     }
        // }

        // 名字换一下, 常用的应该在拓展方法那里, 拓展方法用Append和Group
        // 里面用Chain和Join
        public void Chain(ParallelGroup group)
        {
            if (group != null)
            {
                _orderNodes.EnqueueBack(group);
            }
        }

        public void Join(IUpdateNode node)
        {
            if (node != null)
            {
                if (_orderNodes.TryPeekBack(out ParallelGroup last))
                {
                    last.Parallel(node);
                }
                else
                {
                    Chain(node);
                }
            }
        }

        // public void Group(IEnumerable<TimelineNode> nodes)
        // {
        //     if (nodes == null)
        //         return;
        //
        //     foreach (var node in nodes)
        //     {
        //         Group(node);
        //     }
        // }

        // public void Group(params TimelineNode[] nodes)
        // {
        //     Group(nodes as IEnumerable<TimelineNode>);
        // }

        // public void Group(ParallelGroup group)
        // {
        //     if (group != null)
        //     {
        //         if (_orderNodes.TryPeekBack(out ParallelGroup last))
        //         {
        //             last.Parallel(group);
        //         }
        //         else
        //         {
        //             Chain(group);
        //         }
        //     }
        // }

        // public void Group(IEnumerable<Timeline> timelines)
        // {
        //     foreach (var timeline in timelines)
        //     {
        //         Group(timeline);
        //     }
        // }

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

        public static Timeline Get(IUpdateNode node)
        {
            Timeline timeline = Get();
            timeline.Chain(node);
            return timeline;
        }

        public static Timeline ArrangeInOrder(IEnumerable<IUpdateNode> nodes)
        {
            Timeline result = Get();

            if (nodes != null)
            {
                foreach (var timeline in nodes)
                {
                    result.Chain(timeline);
                }
            }

            return result;
        }

        public static Timeline ArrangeInOrder(params Timeline[] nodes)
        {
            return ArrangeInOrder(nodes as IEnumerable<Timeline>);
        }
    }
}
