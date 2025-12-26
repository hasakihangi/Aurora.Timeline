using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // 提供并行能力
    // 可以代替TimelineNode?
    [System.Serializable]
    public class ParallelGroup
    {
        private List<IUpdateNode> _groupNodes = new List<IUpdateNode>();
        private List<IUpdateNode> _continueNodes = new List<IUpdateNode>();

        public void Parallel(IUpdateNode node)
        {
            _groupNodes.Add(node);
        }

        public void ParallelContinue(IUpdateNode node)
        {
            _continueNodes.Add(node);
        }

        public bool Complete => _groupNodes.Count == 0;
        public bool Continue => _continueNodes.Count > 0;


        public void Update(float delta, float rate)
        {
            for (int i = 0; i < _groupNodes.Count; i++)
            {
                IUpdateNode current = _groupNodes[i];
                current.Update(delta, rate);
                if (current.Complete)
                {
                    _groupNodes.RemoveAt(i);
                    i--;
                    if (current.Continue)
                    {
                        _continueNodes.Add(current);
                    }
                }
            }

            for (int i = 0; i < _continueNodes.Count; i++)
            {
                IUpdateNode current = _continueNodes[i];
                current.Update(delta, rate);
                if (!current.Continue)
                {
                    _continueNodes.RemoveAt(i);
                    i --;
                }
            }
        }



        public static ParallelGroup Get() => new ParallelGroup();
    }
}
