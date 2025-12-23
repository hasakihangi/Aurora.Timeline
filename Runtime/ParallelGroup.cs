using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // 提供并行能力
    [System.Serializable]
    public class ParallelGroup
    {
        [SerializeField] protected List<IUpdateNode> _parallelNodes = new List<IUpdateNode>();

        private ParallelGroup(IUpdateNode node)
        {
            _parallelNodes.Add(node);
        }

        public bool UpdateParallel(float delta, float rate)
        {
            for (int i = _parallelNodes.Count - 1; i >= 0; i--)
            {
                IUpdateNode current = _parallelNodes[i];
                if (current.Update(delta, rate))
                {
                    _parallelNodes.RemoveAt(i);
                }
            }

            if (_parallelNodes.Count == 0)
                return true;

            return false;
        }

        public void Parallel(IUpdateNode node)
        {
            _parallelNodes.Add(node);
        }

        public static ParallelGroup Get(IUpdateNode node) => new ParallelGroup(node);
    }
}
