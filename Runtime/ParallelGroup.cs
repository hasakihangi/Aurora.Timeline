using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    // 提供并行能力
    // 可以代替TimelineNode?
    [System.Serializable]
    public class ParallelGroup
    {
        private List<IUpdateNode> _nodes = new List<IUpdateNode>();
        // private List<IUpdateNode> _continueNodes = new List<IUpdateNode>();

        public void Parallel(IUpdateNode node)
        {
            _nodes.Add(node);
        }

        // ? IUpdateNode自身会决定是否continue
        // public void ParallelContinue(IUpdateNode node)
        // {
        //     _continueNodes.Add(node);
        // }

        public bool Completed
        {
            get
            {
                foreach (var node in _nodes)
                {
                    if (!node.Completed)
                        return false;
                }

                return true;
            }
        }

        public bool Finished => _nodes.Count == 0;


        public void Update(float delta, float rate)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                IUpdateNode current = _nodes[i];
                current.Update(delta, rate);
                if (current.Finished)
                {
                    _nodes.RemoveAt(i);
                    i--;
                }
            }

            // for (int i = 0; i < _continueNodes.Count; i++)
            // {
            //     IUpdateNode current = _continueNodes[i];
            //     current.Update(delta, rate);
            //     if (!current.Finished)
            //     {
            //         _continueNodes.RemoveAt(i);
            //         i --;
            //     }
            // }
        }



        public static ParallelGroup Get() => new ParallelGroup();
    }
}
