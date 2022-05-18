using UnityEngine;

namespace Utilities
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static bool IsPermanent = false;

        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject obj = new GameObject()
                        {
                            name = typeof(T).Name
                        };
                        _instance = obj.AddComponent<T>();
                    }
                    // Debug.Log($"{typeof(T).Name} is Permanent: {IsPermanent.ToString()}");
                    if (IsPermanent) DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }
    }
}