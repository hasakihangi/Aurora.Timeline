using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;

namespace Aurora.Timeline
{
    // 可以分为数据部分和运行时部分, 数据部分 struct 可以重复使用
    // 依然在node这一层进行组织安排, 只要指出最后是哪一点
    // todo: struct包装
    public class TimelineNode
    {
        public TimelineNodeMethod method;
        public float _elapsed = 0f;
        public List<TimelineNode> next = new List<TimelineNode>();
        public float nodeRate = 1f;
        public string name = string.Empty;

        // public Action onDone;
        private Action<TimelineNode> _onComplete;
        private object _callback;
        private object _target;

        public bool Update(float delta, float rate)
        {
            float actualDelta = delta * nodeRate;

            bool done = method == null || method(actualDelta, _elapsed, rate * nodeRate);

            _elapsed += actualDelta;

            if (done && _onComplete != null)
            {
                _onComplete.Invoke(this);
                _onComplete = null;
                _callback = null;
                _target = null;
            }

            return done;
        }

        // public Action OnComplete
        // {
        //     set()
        // }

        public Action OnDone
        {
            set
            {
                if (value == null)
                {
                    _onComplete = null;
                    return;
                }

                _callback = value;
                _onComplete = node =>
                {
                    var callback = node._callback as Action;
                    callback.Invoke();
                };
            }
        }

        // null的逻辑?
        // primeTween的逻辑是如果为null就不赋值? 如果为null应该是取消所有的赋值
        // onComplete的赋值和callback的赋值应该是保持同步的
        public void OnComplete(Action action)
        {
            if (action == null)
            {
                _onComplete = null;
                return;
            }

            _callback = action;
            _onComplete = node =>
            {
                var callback = node._callback as Action;
                callback.Invoke();
            };
        }

        public void OnComplete<T>(T target, Action<T> action) where T: class
        {
            if (action == null)
            {
                _onComplete = null;
                return;
            }

            _callback = action;
            _target = target;

            _onComplete = node =>
            {
                var callback = node._callback as Action<T>;
                var t = _target as T;
                callback.Invoke(t);
            };
        }

        public void AddToNext(TimelineNode node)
        {
            if (node == null)
                return;

            next.Add(node);
        }

        public void AddToNext(Action action)
        {
            TimelineNode node = TimelineNode.DoneAction(action);
            AddToNext(node);
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

        // 有必要改成CompleteAction的命名吗?
        // Done这个命名是否合适?
        public static TimelineNode DoneAction(Action action)
        {
            TimelineNode node = TimelineNode.Get();
            node.OnComplete(action);
            return node;
        }

        public static TimelineNode DoneAction<T>(T target, Action<T> action) where T: class
        {
            TimelineNode node = TimelineNode.Get();
            node.OnComplete<T>(target, action);
            return node;
        }

        public static implicit operator TimelineNode(Action action)
        {
            return DoneAction(action);
        }

        // 确实要捕获局部变量
        // 或者onDone这个本身就是带有一个参数的,
        // 如果要支持这样一个带参数的重载以减少gc, 可以怎么写?
        // 不是很好写, 可以参考primeTween的方案
        // public static TimelineNode DoneAction<T>(Action<T> action)
        // {

        // }

        public static TimelineNode Delay(float seconds)
        {
            TimelineNode node = Get(
                (delta, elapsed, rate) => elapsed >= seconds);
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


        public static TimelineNode ArrangeInOrder(params TimelineNode[] nodes)
        {
            return ArrangeInOrder(nodes as IEnumerable<TimelineNode>);
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

        public static TimelineNode ArrangeInOrder(IEnumerable<TimelineNode> nodes, float interval)
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
            _onComplete?.Invoke(this);
            next.Clear();
        }

        public static TimelineNode Get(TimelineNodeMethod method)
        {
            TimelineNode node = Get();
            node.method = method;
            return node;
        }

        public static TimelineNode Get(Action doneAction)
        {
            return TimelineNode.DoneAction(doneAction);
        }

        public static TimelineNode Get(TimelineNodeMethod method, Action doneAction)
        {
            TimelineNode node = Get();
            node.method = method;
            node.OnComplete(doneAction);
            return node;
        }

        public static TimelineNode Get()
        {
            return new TimelineNode();
        }

        private TimelineNode() {}
    }

    public delegate bool TimelineNodeMethod(float delta, float elapsed, float rate);
}
