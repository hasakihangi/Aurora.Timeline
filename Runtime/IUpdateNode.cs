using System;

namespace Aurora.Timeline
{
    public interface IUpdateNode
    {
        // public bool Update(float delta, float rate);
        public void Update(float delta, float rate);
        public bool Complete {get;}
        public bool Continue {get;} // 直到Continue变为false
    }
}
