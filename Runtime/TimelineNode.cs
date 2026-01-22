using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    public class TimelineNode
    {
        public UpdateNode updateNode;
        public List<TimelineNode> _next = new List<TimelineNode>();

        private TimelineNode()
        {

        }

        private TimelineNode(UpdateNode updateNode)
        {
            this.updateNode = updateNode;
        }

        public Action OnComplete
        {
            get => updateNode.onCompleted;
            set => updateNode.onCompleted = value;
        }

        public void AddToNext(TimelineNode node)
        {
            if (node == null)
                return;

            _next.Add(node);
        }

        public static TimelineNode Nothing => new TimelineNode();
        public static TimelineNode Action(Action action) => new TimelineNode(UpdateNode.ActionNode(action));
        public static TimelineNode Done(Action onDone) => new TimelineNode(UpdateNode.DoneNode(onDone));
        public static TimelineNode Delay(float seconds) => new TimelineNode(UpdateNode.DelayNode(seconds));

        public static TimelineNode Delay(float seconds, Action onDone)
        {
            TimelineNode n = new TimelineNode(UpdateNode.DelayNode(seconds));
            n.OnComplete = onDone;
            return n;
        }



        public static TimelineNode ArrangeInOrder(IEnumerable<TimelineNode> nodes)
        {
            TimelineNode first = null;
            TimelineNode node = null;
            foreach (var n in nodes)
            {
                if (n == null) continue;

                if (first == null)
                {
                    first = n;
                    node = first;
                }
                else
                {
                    node.AddToNext(n);
                    node = n;
                }
            }

            return first ?? Nothing;
        }

        public static TimelineNode ArrangeInOrder(params TimelineNode[] nodes)
        {
            return ArrangeInOrder(nodes as IEnumerable<TimelineNode>);
        }

        public static TimelineNode ArrangeNodesInParallel(IEnumerable<TimelineNode> nodes)
        {
            TimelineNode emptyStartNode = Nothing;
            foreach (var node in nodes)
            {
                emptyStartNode.AddToNext(node);
            }
            return emptyStartNode;
        }

        public static TimelineNode ArrangeNodesInParallel(params TimelineNode[] nodes)
        {
            return ArrangeNodesInParallel(nodes as IEnumerable<TimelineNode>);
        }

        public static TimelineNode Get(UpdateNode node)
        {
            return new TimelineNode(node);
        }

        public static TimelineNode Get(UpdateMethod method)
        {
            return Get(new UpdateNode(method));
        }

        public static TimelineNode Get() => new TimelineNode();
    }
}
