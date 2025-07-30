using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;

namespace Aurora.Timeline
{
    // 可以分为数据部分和运行时部分, 数据部分 struct 可以重复使用
    public class TimelineNode
    {
        public TimelineNodeMethod method;
        public float m_Elapsed = 0f;
        public List<TimelineNode> next = new List<TimelineNode>();
        public float nodeRate = 1f;
        public Action onDone;
        public string name = string.Empty;

        public bool Update(float delta, float rate)
        {
            float actualDelta = delta * nodeRate;

            bool done = method == null || method(actualDelta, m_Elapsed, rate * nodeRate);

            m_Elapsed += actualDelta;

            if (done && onDone != null)
            {
                onDone.Invoke();
                onDone = null;
            }

            return done;
        }

        public void AddToNext(TimelineNode node)
        {
            if (node == null)
                return;

            next.Add(node);
        }

        public void AddToNext(IEnumerable<TimelineNode> nodes)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                AddToNext(node);
            }
        }

        public static TimelineNode Nothing
        {
            get
            {
                TimelineNode nothing = TimelineNode.Get();
                nothing.name = "nothing";
                return nothing;
            }
        }

        public static TimelineNode WaitSeconds(float sec)
        {
            TimelineNode node = Get((delta, elapsed, rate) => elapsed >= sec);
            node.name = "wait secondes";
            return node;
        }

        public static TimelineNode Action(Action action) => Get((delta, elapsed, rate) =>
            {
                action?.Invoke();
                return true;
            }
        );

        public static TimelineNode DoneAction(Action action)
        {
            TimelineNode node = TimelineNode.Get();
            node.onDone = action;
            return node;
        }


        public static TimelineNode Delay(float seconds, Action callback)
        {
            TimelineNode node = Get(
                (delta, elapsed, rate) => elapsed >= seconds,
                callback);
            node.name = "delay onDoneAction";
            return node;
        }


        public static TimelineNode ArrangeNodesInOrder(params TimelineNode[] nodes)
        {
            return ArrangeNodesInOrder(nodes as IEnumerable<TimelineNode>);
        }

        public static TimelineNode ArrangeNodesInOrder(IEnumerable<TimelineNode> nodes)
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

            return first;
        }

        public static TimelineNode ArrangeNodesInOrder(IEnumerable<TimelineNode> nodes, float interval)
        {
            if (nodes == null) return null;

            TimelineNode first = null;
            TimelineNode current = null;

            foreach (var node in nodes)
            {
                if (node == null) continue;

                if (first == null)
                {
                    first = node;
                    current = first;
                }
                else
                {
                    if (interval > 0f)
                    {
                        TimelineNode waitNode = WaitSeconds(interval);
                        current.AddToNext(waitNode);
                        current = waitNode;
                    }

                    current.AddToNext(node);
                    current = node;
                }
            }

            return first;
        }

        public static TimelineNode ArrangeNodesInParallel(params TimelineNode[] nodes)
        {
            if (nodes.Length == 0) return null;
            TimelineNode emptyStartNode = Nothing;
            foreach (var node in nodes)
            {
                emptyStartNode.AddToNext(node);
            }

            return emptyStartNode;
        }

        public void Clear()
        {
            onDone?.Invoke();
            next.Clear();
        }

        public static TimelineNode Get(TimelineNodeMethod method)
        {
            TimelineNode node = Get();
            node.method = method;
            return node;
        }

        public static TimelineNode Get(Action onDoneAction)
        {
            TimelineNode node = Get();
            node.onDone = onDoneAction;
            return node;
        }

        public static TimelineNode Get(TimelineNodeMethod method, Action onDoneAction)
        {
            TimelineNode node = Get();
            node.method = method;
            node.onDone = onDoneAction;
            return node;
        }

        public static TimelineNode Get()
        {
            return new TimelineNode();
        }

        private TimelineNode() {}
    }

    public delegate bool TimelineNodeMethod(float delta, float elapsed, float rate);

//
// public delegate void TimelineNodeDebugMethod(TimelineNode node)
// 目前就用node, 能做到
}
