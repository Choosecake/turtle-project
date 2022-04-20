using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    [RequireComponent(typeof(FeedingSystem))]
    [RequireComponent(typeof(BreathingSystem))]
    public class TurtleVitalSystems : MonoBehaviour, IVitalSystems
    {
        public IEnumerable Die()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield return null;
        }
    }
}