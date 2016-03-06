using UnityEngine;
using System.Collections;

public class PlayerColorManager : MonoBehaviour {

    public Color[] colors;
    public static PlayerColorManager Instance
    {
        get;
        private set;
    }

    public Color GetColor(int index)
    {
        return colors[index];
    }

    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }
}
