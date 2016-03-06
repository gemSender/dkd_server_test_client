using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour {
    private long lastSyncPosTime;
    public float Speed;
    static Dictionary<string, Player> playerDict = new Dictionary<string, Player>();
    private NavMeshAgent agent;
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

    public static Player Create(string id, float x, float z, bool isMine, int colorIndex, long timestamp) {
        var go = Instantiate(Resources.Load("Player")) as GameObject;
        var goTrans = go.transform;
        goTrans.localPosition = new Vector3(x, 0.5f, z);
        var player = goTrans.GetComponent<Player>();
        player.Id = id;
        player.IsMine = isMine;
        playerDict[id] = player;
        player.lastSyncPosTime = timestamp;
        player.GetComponent<Renderer>().material.color = PlayerColorManager.Instance.GetColor(colorIndex);
        return player;
    }
	void Start () {
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
	}

    public static Player GetPlayer(string id) {
        try
        {
            return playerDict[id];
        }
        catch (System.Exception e){
            Debug.Log("key not found: " + id);
            throw e;
        }
    }
    public static void RemovePlayer(string id){
        var player = playerDict[id];
        playerDict.Remove(id);
        Destroy(player.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (IsMine) {
            if (Input.GetMouseButtonDown(1)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"))) {
                    var hitPos = hit.point;
                    agent.SetDestination(hitPos);
                    state = State.Moving;
                }
            }
            var now = ConnectionHandler.Instance.CurrentTimeMS;
            int deltaTimeMS = (int)(now - lastSyncPosTime);
            if (deltaTimeMS > 300)
            {
                var pos = transform.localPosition;
                var forward = transform.forward;
                ConnectionHandler.Instance.SendUnpackedMessage(messages.MoveTo.CreateBuilder()
                    .SetX(pos.x).SetY(pos.z).SetDirX(forward.x).SetDirY(forward.z).SetTimestamp(now).Build());
                lastSyncPosTime = now;
            }
        }
        switch (state) { 
            case State.Idle:
                break;
            case State.Moving:
                break;
        }
	}

    public void MoveTo(float x, float y, float xDir, float yDir, long timestamp) {
        var currentPos = transform.localPosition;
        currentPos.x = x;
        currentPos.z = y;
        transform.localPosition = currentPos;
        transform.forward = new Vector3(xDir, 0 , yDir);
    }

    void OnGUI() {
        if (IsMine) {
            GUILayout.Button(this.Id);
        }
    }
}
