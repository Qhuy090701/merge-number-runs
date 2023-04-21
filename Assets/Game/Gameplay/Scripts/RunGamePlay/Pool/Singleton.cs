using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();

                if(instance == null)
                {
                    CreateSingletonObject();
                }
            }

            return instance;
        }
    }

    private static void CreateSingletonObject()
    {
        GameObject gameObject = new GameObject(typeof(T).Name);
        instance = gameObject.AddComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}
