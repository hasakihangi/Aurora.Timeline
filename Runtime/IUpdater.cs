using System;

namespace Aurora.Timeline
{
    public interface IUpdater
    {
        public bool Update(float delta, float rate);
    }
}
