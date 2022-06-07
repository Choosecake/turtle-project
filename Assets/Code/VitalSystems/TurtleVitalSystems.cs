using System;
using System.Collections;
using Code.DeathMessages;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    [RequireComponent(typeof(FeedingSystem))]
    [RequireComponent(typeof(BreathingSystem))]
    public class TurtleVitalSystems : MonoBehaviour, IVitalSystems, GameEnder
    {
        public Action OnTurtleDeath;

        private bool _isDead = false;
        
        public IEnumerable Die(CauseOfDeath causeOfDeath)
        {
            if (_isDead) yield break;
            
            OnTurtleDeath?.Invoke();
            OnCriticalPointReached?.Invoke(causeOfDeath);
            _isDead = true;
            yield return null;
        }

        public bool IsDead => _isDead;
        public Action<CauseOfDeath> OnCriticalPointReached { get; set; }
    }
}