using UnityEngine;


namespace Kit
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : Component
    { 
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<T>();
                    return _instance;
                }

                return _instance;
            }
        } 
        private static T _instance;
 
        public virtual void Awake()
        {
            DontDestroyOnLoad ( this.gameObject );  
        }
        
        
    }
}