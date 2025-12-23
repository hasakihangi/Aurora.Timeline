using System;

namespace Aurora.Timeline
{
    // 感觉还要提供取消或者完成的方法
    // Timeline这些就只有完成方法, 只有updateNode和TimelineNode有Cancel方法
    // 因为onDone是可选的, 所以目前只需要Complete逻辑, 最终都传递到updateNode上面, 让updateNode自己完成?
    // timelineNode是树状结构, cancel表示只取消当前的表现?
    // 目前为什么需要完成方法? 因为计算calculateNode 取消这个timeline? 直接进行标记, 然后在Update里面直接返回true就行了, 需要哪里才标记哪里, 不乱加tag
    public interface IUpdateNode
    {
        public bool Update(float delta, float rate);
        // public bool Cancel{set;}
        // public bool Complete{set;}
    }
}
