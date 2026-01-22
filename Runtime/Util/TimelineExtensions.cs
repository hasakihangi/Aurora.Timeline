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


        public static Timeline Append(this Timeline t, IUpdateNode node)
        {
            if (t == null)
                t = Timeline.Get();

            if (node == null)
                return t;

            t.Chain(node);
            return t;
        }

        // public static Timeline AppendContinue(this Timeline t, IUpdateNode node)
        // {
        //     if (t == null)
        //         t = Timeline.Get();
        //
        //     if (node == null)
        //         return t;
        //
        //     t.ChainContinue(node);
        //     return t;
        // }

        public static Timeline Append(this Timeline t, UpdateMethod method)
        {
            UpdateNode node = new UpdateNode(method);
            return Append(t, node);
        }

        public static Timeline Append(this Timeline t, Action onDone)
        {
            UpdateNode node = new UpdateNode(onDone);
            return Append(t, node);
        }

        public static Timeline Group(this Timeline t, IUpdateNode node)
        {
            if (t == null)
                t = Timeline.Get();

            if (node == null)
                return t;

            t.Join(node);
            return t;
        }

        // public static Timeline GroupContinue(this Timeline t, IUpdateNode node)
        // {
        //     if (t == null)
        //         t = Timeline.Get();
        //
        //     if (node == null)
        //         return t;
        //
        //     t.JoinContinue(node);
        //     return t;
        // }

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


        public static Timeline ToTimeline(this TimelineNode node)
        {
            Timeline timeline = Timeline.Get();

            if (node == null)
                return timeline;

            TimelineNodeUpdater updater = TimelineNodeUpdater.Get(node);
            timeline.Join(updater);

            return timeline;
        }

        // 这里是在干什么?
        // 最好是Timeline提供这个Delay方法, 然后再Group
        // public static Timeline DelayGroup(this Timeline t, float seconds, IUpdateNode node)
        // {
        //     t ??= Timeline.Get();
        //
        //     if (node == null)
        //         return t;
        //
        //     Timeline result = Timeline.Get();
        //
        //     UpdateNode delay = UpdateNode.DelayNode(seconds);
        //
        //     result.Append(delay).Append(node);
        //     t.Join(node); // 有问题吧?
        //     return t;
        // }
        //
        // public static Timeline DelayGroup(this Timeline t, float seconds, TimelineNode node)
        // {
        //     t ??= Timeline.Get();
        //
        //     if (node == null)
        //         return t;
        //
        //     TimelineNode delay = TimelineNode.Delay(seconds);
        //     delay.AddToNext(node);
        //     t.Group(delay);
        //     return t;
        // }

        // public static Timeline Delay(this Timeline t, float seconds)
        // {
        //     t ??= Timeline.Get();
        //
        //     TimelineNode node = TimelineNode.Delay(seconds);
        //     return t.Append(node);
        // }
    }
}
