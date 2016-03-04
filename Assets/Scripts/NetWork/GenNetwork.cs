using UnityEngine;
using System.Collections;

public class GenNetwork<T> : MonoBehaviour where T : GenNetwork<T>{
    public static T Instance
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }else{
            Instance = this as T;
        }
        
    }
	// Use this for initialization
	protected virtual void Start () {
        
	}

    protected virtual void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }
}
