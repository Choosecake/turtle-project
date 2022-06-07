using System.Collections;
using Code.DeathMessages;
using UnityEngine.EventSystems;

namespace Code
{
    public interface IVitalSystems : IEventSystemHandler
    {
        public IEnumerable Die(CauseOfDeath causeOfDeath);
    }
}