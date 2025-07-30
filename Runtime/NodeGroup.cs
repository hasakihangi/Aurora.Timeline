
using System.Collections.Generic;

namespace Aurora.Timeline
{
    public class NodeGroup: ParallelNode
    {
        private List<TimelineNode> nodes = new List<TimelineNode>();

        public override bool Update(float delta, float rate)
        {
            bool parallelDone = UpdateParallel(delta, rate);
            bool nodesDone = UpdateNodes(delta, rate);
            return parallelDone && nodesDone;
        }

        private bool UpdateNodes(float delta, float rate)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                TimelineNode current = nodes[i];
                if (current.Update(delta, rate))
                {
                    nodes.RemoveAt(i);
                    if (current.next.Count > 0)
                    {
                        nodes.AddRange(current.next);
                    }
                }
            }

            if (nodes.Count == 0)
                return true;

            return false;
        }

        public override void Parallel(TimelineNode node)
        {
            nodes.Add(node);
        }

        private NodeGroup() {}

        public static NodeGroup Get()
        {
            NodeGroup group = new NodeGroup();
            return group;
        }
    }
}

