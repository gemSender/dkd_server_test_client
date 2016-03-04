using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private float lastSyncPosTime;
    public float Speed;
    public string Id
    {
        get;
        set;
    }

    public bool IsMine
    {
        get;
        set;
    }

    enum State {
        Idle,
        Moving
    }
    State state;
	// Use this for initialization
	void Start () {
        state = State.Idle;
	}
	
	// Update is called once per frame
	void Update () {
        if (IsMine) {
            if (Input.GetMouseButton(1)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("Ground"))) {
                    var hitPos = hit.point;
                }
            }
        }
        switch (state) { 
            case State.Idle:
                break;
            case State.Moving:
                break;
        }
	}

    public void MoveTo(float x, float y, float xDir, float yDir) {
        var currentPos = transform.localPosition;
        currentPos.x = x;
        currentPos.y = y;
        transform.localPosition = currentPos;
        transform.forward = new Vector3(xDir, yDir, 0);
    }
}
