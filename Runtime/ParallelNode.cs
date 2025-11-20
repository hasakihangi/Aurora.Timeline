using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    [System.Serializable]
    public abstract class ParallelNode
    {
        [SerializeField] protected List<ParallelNode> parallelNodes = new List<ParallelNode>();
        public bool UpdateParallel(float delta, float rate)
        {
            for (int i = parallelNodes.Count - 1; i >= 0; i--)
            {
                ParallelNode current = parallelNodes[i];
                if (current.Update(delta, delta))
                {
                    parallelNodes.RemoveAt(i);
                }
            }

            if (parallelNodes.Count == 0)
                return true;

            return false;
        }

        public void Parallel(ParallelNode node)
        {
            parallelNodes.Add(node);
        }

        public abstract bool Update(float delta, float rate);
        public abstract void Parallel(TimelineNode node);

        public abstract string ToString(int level);
    }
}
