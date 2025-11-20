
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aurora.Timeline
{
    [System.Serializable]
    public class NodeGroup: ParallelNode
    {
        [SerializeField] public List<TimelineNode> nodes = new List<TimelineNode>();

        public override bool Update(float delta, float rate)
        {
            bool parallelDone = UpdateParallel(delta, rate);
            bool nodesDone = UpdateNodes(delta, rate);
            return parallelDone && nodesDone;
        }

        public override string ToString(int level)
        {
            string output = new string('\t', level);
            for (int i = 0; i < nodes.Count; i++)
            {
                output += $"{i}:{nodes[i]._name}  ";
            }
            return output;
        }

        public IEnumerable<TimelineNode> FindNodes(string tag)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].IsMatchTag(tag))
                {
                    yield return nodes[i];
                }
            }
        }

        private bool UpdateNodes(float delta, float rate)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                TimelineNode current = nodes[i];
                if (current.Update(delta, rate))
                {
                    nodes.RemoveAt(i);
                    if (current._next.Count > 0)
                    {
                        nodes.AddRange(current._next);
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

