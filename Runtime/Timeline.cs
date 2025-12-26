using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    [Serializable]
    public class Timeline: IUpdateNode
    {
        private List<ParallelGroup> _order = new List<ParallelGroup>();

        public float _rate = 1;
        public string _name = string.Empty;

        // public int _index = 0;

        public bool Complete {get; private set; }
        public bool Continue => _order.Count > 0;

        public void Update(float delta, float rate)
        {
            for (int i = 0; i < _order.Count; i++)
            {
                var group = _order[i];
                group.Update(delta * _rate, rate * _rate);

                if (group.Complete)
                {
                    if (!group.Continue)
                    {
                        _order.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    return;
                }
            }

            Complete = true;
        }


        public void Cancel()
        {
            _order.Clear();
        }


        public void Parallel(IUpdateNode node)
        {
            if (node == null)
                return;

            if (_order.TryGetValue(0, out var group))
            {
                group.Parallel(node);
            }
            else
            {
                Chain(node);
            }
        }

        public void Chain(IUpdateNode node)
        {
            if (node == null)
                return;

            ParallelGroup group = ParallelGroup.Get();
            group.Parallel(node);
            _order.Add(group);
        }

        public void ChainContinue(IUpdateNode node)
        {
            if (node == null)
                return;

            ParallelGroup group = ParallelGroup.Get();
            group.ParallelContinue(node);
            _order.Add(group);
        }

        public void Join(IUpdateNode node)
        {
            if (node == null)
                return;

            if (_order.Count > 0)
            {
                _order[^1].Parallel(node);
            }
            else
            {
                Chain(node);
            }
        }

        public void JoinContinue(IUpdateNode node)
        {
            if (node == null)
                return;

            if (_order.Count > 0)
            {
                _order[^1].ParallelContinue(node);
            }
            else
            {
                ChainContinue(node);
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
