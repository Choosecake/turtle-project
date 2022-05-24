using System;

namespace UI
{
    public interface GameEnder
    {
        public Action OnCriticalPointReached { get; set; }
    }
}