using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Managers
{
    [CreateAssetMenu(fileName = "GeneralDataStorage", menuName = "General Data Storage")]
    public class GeneralDataStorageSO : ScriptableObject
    {
        private int currentLevel = 0;

        public int CurrentLevel
        {
            set => currentLevel = Mathf.Clamp(value, 1, SceneManager.sceneCountInBuildSettings);
            get => currentLevel;
        }
    }
}