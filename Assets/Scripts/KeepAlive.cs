using UnityEngine;
using System.Collections;

public class KeepAlive : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        yield return null;
        var components = GetComponents<MonoBehaviour>();
        if (components.Length <= 1)
        {
            Destroy(gameObject);
        }
    }
}
