using System;
using UnityEngine;

namespace Aurora.Timeline
{
    internal class SingletonBehaviour<T>: MonoBehaviour where T: SingletonBehaviour<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance is null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
    }
}
