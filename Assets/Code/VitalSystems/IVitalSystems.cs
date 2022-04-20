using System.Collections;
using UnityEngine.EventSystems;

namespace Code
{
    public interface IVitalSystems : IEventSystemHandler
    {
        public IEnumerable Die();
    }
}