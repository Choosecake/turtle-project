using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "General Data Storage", menuName = "General Data Storage")]
    public class GeneralDataStorageSO : ScriptableObject
    {
        public int currentLevel = 0;
    }
}