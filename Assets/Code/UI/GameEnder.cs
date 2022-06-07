using System;
using Code.DeathMessages;

namespace UI
{
    public interface GameEnder
    {
        public Action<CauseOfDeath> OnCriticalPointReached { get; set; }
    }
}