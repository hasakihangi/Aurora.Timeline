using System.Collections.Generic;

// 经常更改的Gameplay部分不搞缓存池
namespace Aurora.Timeline
{
    // 指定哪一个算完成?
    public abstract class ParallelNode
    {
        protected List<ParallelNode> parallelNodes = new List<ParallelNode>();
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


    }
}
