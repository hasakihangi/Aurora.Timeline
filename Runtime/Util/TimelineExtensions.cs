using System;

namespace Aurora.Timeline
{
    public static class TimelineExtensions
    {
        public static void Register(this Track track)
        {
            TrackManager.Instance.Register(track);
        }

        public static void Unregister(this Track track)
        {
            TrackManager.Instance.Unregister(track);
        }

        // Append始终是转成TimelineNodeGroup的形式加入
        // 但是如果是Group呢? 需要一个IParallel的接口? 但是TimelineNodeGroup没办法跟其他IUpdate对象共存
        public static Timeline Append(this Timeline t, TimelineNode node)
        {
            if (t == null)
                t = Timeline.Get();

            if (node == null)
                return t;

            TimelineNodeUpdater updater = TimelineNodeUpdater.Get(node);
            t.Chain(updater);
            return t;
        }


        public static Timeline Append(this Timeline t, IUpdater node)
        {
            if (t == null)
                t = Timeline.Get();

            t.Chain(node);
            return t;
        }

        public static Timeline Append(this Timeline t, UpdateMethod method)
        {
            UpdateNode node = new UpdateNode(method);
            return Append(t, node);
        }

        // public static Timeline Append(this Timeline t, params Timeline[] timelines)
        // {
        //     if (t == null)
        //         t = Timeline.Get();
        //
        //     t.Chain(timelines);
        //     return t;
        // }

        public static Timeline Append(this Timeline t, Action onDone)
        {
            UpdateNode node = new UpdateNode(onDone);
            return Append(t, node);
        }

        public static Timeline Group(this Timeline t, IUpdater node)
        {
            t ??= Timeline.Get();
            t.Join(node);
            return t;
        }

        public static Timeline Group(this Timeline t, Action onDone)
        {
            UpdateNode node = new UpdateNode(onDone);
            return Group(t, node);
        }


        public static Timeline Group(this Timeline t, TimelineNode node)
        {
            t ??= Timeline.Get();

            if (node == null)
                return t;

            TimelineNodeUpdater updater = TimelineNodeUpdater.Get(node);
            t.Join(updater);
            return t;
        }


        // public static TimelineGroupGroup ToNodeGroup(this TimelineNode n)
        // {
        //     TimelineGroupGroup groupGroup = TimelineGroupGroup.Get();
        //     groupGroup.Parallel(n);
        //     return groupGroup;
        // }

        public static Timeline ToTimeline(this TimelineNode node)
        {
            Timeline timeline = Timeline.Get();

            if (node == null)
                return timeline;

            TimelineNodeUpdater updater = TimelineNodeUpdater.Get(node);
            timeline.Join(updater);

            return timeline;
        }

        //现在需要的是Delay多少秒之后执行一个Timeline
        public static Timeline DelayGroup(this Timeline t, float seconds, IUpdater node)
        {
            t ??= Timeline.Get();

            if (node == null)
                return t;

            Timeline result = Timeline.Get();

            // ? 这个还真得使用TimelineNode才能实现? 不是, 可以用timeline, timeline也是IUpdateNode对象
            // TimelineNode delayNode = TimelineNode.Delay(seconds);
            UpdateNode delay = UpdateNode.Delay(seconds);

            result.Append(delay).Append(node);
            t.Join(node);
            return t;
        }

        public static Timeline DelayGroup(this Timeline t, float seconds, TimelineNode node)
        {
            t ??= Timeline.Get();

            if (node == null)
                return t;

            TimelineNode delay = TimelineNode.Delay(seconds);
            delay.AddToNext(node);
            t.Group(delay);
            // TimelineNode node = TimelineNode.Delay(seconds);
            // node.AddToNext(afterDelay);
            // t.Group(node);
            return t;
        }

        public static Timeline Delay(this Timeline t, float seconds)
        {
            t ??= Timeline.Get();

            TimelineNode node = TimelineNode.Delay(seconds);
            return t.Append(node);
        }
    }
}
