using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.DeathMessages
{
    [Serializable]
    public class DeathMessageManager
    {
        [SerializeField] private DeathMessageSO[] deathMessages;

        public DeathMessageSO GetMessage(CauseOfDeath causeOfDeath)
        {
            if (causeOfDeath == CauseOfDeath.Default)
            {
                return deathMessages[0];
            }

            DeathMessageSO[] filteredArray = 
                deathMessages.Where(dM => dM.causeOfDeath == causeOfDeath).ToArray();
            if (filteredArray.Length == 0)
            {
                return deathMessages[0];
            }
            return filteredArray.ElementAt(Random.Range(0, filteredArray.Count()));
        }
    }
}