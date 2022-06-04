using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Utilities
{
    public static class MonoUtilities
    {
        public static List<T> FindGameObjects<T>(bool includeInactive = false)
        {
            List<T> objects = new List<T>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenObjects = rootGameObject.GetComponentsInChildren<T>(includeInactive);
                foreach (var childObject in childrenObjects)
                {
                    objects.Add(childObject);
                }
            }

            return objects;
        }

        public static bool TryFindWithTag(out GameObject gameObject, string tag)
        {
            gameObject = GameObject.FindGameObjectWithTag(tag);
            return gameObject != null;
        }
    }
}