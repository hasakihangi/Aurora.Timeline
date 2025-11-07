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

        public static Timeline Append(this Timeline timeline, TimelineNode node)
        {
            timeline.Chain(node);
            return timeline;
        }


        public static Timeline Append(this Timeline t, Timeline timeline)
        {
            t.Chain(timeline);
            return t;
        }

        public static Timeline Append(this Timeline t, TimelineNodeMethod method)
        {
            TimelineNode node = TimelineNode.Get(method);
            t.Append(node);
            return t;
        }

        public static Timeline Append(this Timeline t, params Timeline[] timelines)
        {
            t.Chain(timelines);
            return t;
        }

        public static Timeline Append(this Timeline t, Action doneAction)
        {
            TimelineNode doneNode = TimelineNode.DoneAction(doneAction);
            t.Append(doneNode);
            return t;
        }

        public static Timeline Join(this Timeline t, Action doneAction)
        {
            TimelineNode node = TimelineNode.DoneAction(doneAction);
            t.Group(node);
            return t;
        }


        public static Timeline Join(this Timeline t, TimelineNode node)
        {
            t.Group(node);
            return t;
        }

        public static Timeline Join(this Timeline t, Timeline timeline)
        {
            t.Group(timeline);
            return t;
        }

        public static NodeGroup ToNodeGroup(this TimelineNode n)
        {
            NodeGroup group = NodeGroup.Get();
            group.Parallel(n);
            return group;
        }

        //现在需要的是Delay多少秒之后执行一个Timeline
        public static Timeline GroupDelay(this Timeline t, float seconds, Timeline afterDelay)
        {
            Timeline timeline = Timeline.Get();
            TimelineNode delayNode = TimelineNode.Delay(seconds);
            timeline.Append(delayNode).Append(afterDelay);
            t.Group(timeline);
            return t;
        }

        public static Timeline GroupDelay(this Timeline t, float seconds, TimelineNode afterDelay)
        {
            TimelineNode node = TimelineNode.Delay(seconds);
            node.AddToNext(afterDelay);
            t.Group(node);
            return t;
        }

        public static Timeline AppendDelay(this Timeline t, float seconds)
        {
            TimelineNode node = TimelineNode.Delay(seconds);
            return t.Append(node);
        }
    }
}
