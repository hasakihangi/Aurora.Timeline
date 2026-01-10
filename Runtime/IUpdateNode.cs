using System;

namespace Aurora.Timeline
{
    public interface IUpdateNode
    {
        public void Update(float delta, float rate);
        public bool Complete {get;}
        public bool Continue {get;}
    }
}
