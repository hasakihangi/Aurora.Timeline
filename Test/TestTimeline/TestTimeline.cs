using System;
using System.Collections;
using System.Collections.Generic;
using Aurora.Timeline;
using UnityEngine;

public class TestTimeline : MonoBehaviour
{
    public Track track = new Track();

    private void Awake()
    {
        track.Register(); // 注册到管理器中
    }

    // Start is called before the first frame update
    void Start()
    {
        track.Run(Test1());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 树形结构测试
    Timeline Test0()
    {
        // 主分支
        Timeline timeline = Timeline.Get();
        timeline.Delay(1f);
        timeline.Append(() => Debug.Log(0));

        // 分支1
        Timeline t1 = Timeline.Get();
        t1.Delay(1f);
        t1.Append(() => Debug.Log(1));
        timeline.Append(t1);

        // 分支2
        Timeline t2 = Timeline.Get();
        t2.Delay(1f).Append(() => Debug.Log(2));
        timeline.Group(t2); // 与分支1并行

        // 分支3
        Timeline t3 = Timeline.Get();
        t3.Delay(1f).Append(() => Debug.Log(3)).Delay(1f).Append(() => Debug.Log(3_1)); // 总共等待2s
        timeline.Group(t3);

        // 3个分支全部执行完成之后
        timeline.Append(() => Debug.Log("test0 complete!"));

        return timeline;
    }

    // Continue测试(Continue表示执行完毕后后续执行的一些表现)
    // 这里的执行完毕不仅可以是一个点, 也可以是一棵树
    Timeline Test1()
    {
        Timeline timeline = Timeline.Get();
        Timeline t0 = Test0();
        Timeline t0_continue = Timeline.Get().Delay(2f).Append(() => Debug.Log("test0 continue"));
        t0.AppendContinue(t0_continue); // 在t0之后执行一些后续的continue, 但是不影响它的complete

        Timeline t1 = Timeline.Get();
        t1.Append(() => Debug.Log("test1 start")).Delay(3f).Append(() => Debug.Log("test1 complete!"));

        timeline.Append(t0).Append(t1); // t1将在t0的complete之后执行
        return timeline;
    }
}
