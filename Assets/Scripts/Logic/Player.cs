using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour {
    private long lastSyncPosTime;
    public float Speed;
    static Dictionary<int, Player> playerDict = new Dictionary<int, Player>();
    private NavMeshAgent agent;
    public messages.StartPathReply serverPath;
    public Vector3[] clientPath;
    public int Index
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

    public static Player Create(int index, float x, float z, bool isMine, int colorIndex, long timestamp) {
        var go = Instantiate(Resources.Load("Player")) as GameObject;
        var goTrans = go.transform;
        goTrans.localPosition = new Vector3(x, 0.5f, z);
        var player = goTrans.GetComponent<Player>();
        player.Index = index;
        player.IsMine = isMine;
        playerDict[index] = player;
        player.lastSyncPosTime = timestamp;
        player.GetComponent<Renderer>().material.color = PlayerColorManager.Instance.GetColor(colorIndex);
        return player;
    }
	void Start () {
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
	}

    public static Player GetPlayer(int index) {
        try
        {
            return playerDict[index];
        }
        catch (System.Exception e){
            Debug.Log("key not found: " + index);
            throw e;
        }
    }
    public static void RemovePlayer(int index){
        var player = playerDict[index];
        playerDict.Remove(index);
        Destroy(player.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (IsMine) {
            var now = ConnectionHandler.Instance.CurrentTimeMS;
            bool leftBtnDown = Input.GetMouseButtonDown(0);
            bool rightBtnDown = Input.GetMouseButtonDown(1);
            if (leftBtnDown || rightBtnDown)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"))) {
                    var hitPos = hit.point;
                    if (rightBtnDown)
                    {
                        agent.SetDestination(hitPos);
                        state = State.Moving;
                        var currentPos = transform.localPosition;
                        currentPos.y = 0;
                        ConnectionHandler.Instance.SendUnpackedMessage<messages.StartPathReply>(messages.StartPath.CreateBuilder()
                            .SetDx(hitPos.x).SetDy(hitPos.z).SetSx(currentPos.x).SetSy(currentPos.z).SetTimestamp(ConnectionHandler.Instance.CurrentTimeMS).Build(),
                            messages.StartPathReply.ParseFrom,
                            (reply) =>
                            {
                                this.serverPath = reply;
                            });
                        int startTri = NavmeshUtility.GetTriangleIndex(new Vector2(currentPos.x, currentPos.z));
                        int endTri = NavmeshUtility.GetTriangleIndex(new Vector2(hitPos.x, hitPos.z));
                        NavMeshPath nvp = new NavMeshPath();
                        agent.CalculatePath(hitPos, nvp);
                        Debug.Log("path:");
                        clientPath = nvp.corners;
                    }
                    else {
                        transform.position = hitPos;
                        agent.SetDestination(hitPos);
                        var forward = transform.forward;
                        ConnectionHandler.Instance.SendUnpackedMessage(messages.MoveTo.CreateBuilder()
                            .SetX(hitPos.x)
                            .SetY(hitPos.z)
                            .SetDirX(forward.x)
                            .SetDirY(forward.z)
                            .SetTimestamp(now).Build());
                    }
                }
            }
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
        if (this.serverPath != null)
        {
            for (int i = 1, imax = this.serverPath.VerticesCount; i < imax; i++)
            {
                var p1 = this.serverPath.GetVertices(i - 1);
                var p2 = this.serverPath.GetVertices(i);
                Debug.DrawLine(new Vector3(p1.X, 0, p1.Y), new Vector3(p2.X, 0, p2.Y), Color.blue);
            }
            for (int i = 0, imax = this.serverPath.EdgesCount; i < imax; i++) { 
                var edge = this.serverPath.GetEdges(i);
                var from = new Vector3(edge.Start.X, 0, edge.Start.Y);
                var to = new Vector3(edge.End.X, 0, edge.End.Y);
                Debug.DrawLine(from, Vector3.Lerp(from, to, 0.8f), Color.yellow);
                Debug.DrawLine(Vector3.Lerp(from, to, 0.8f), to, Color.white);
            }
        }
        if (clientPath != null)
        {
            for (int i = 1; i < clientPath.Length; i++) {
                var p1 = clientPath[i - 1];
                var p2 = clientPath[i];
                Debug.DrawLine(p1, p2, Color.green);
            }
        }
	}

    public void MoveTo(float x, float y, float xDir, float yDir, long timestamp) {
        var currentPos = transform.localPosition;
        currentPos.x = x;
        currentPos.z = y;
        transform.localPosition = currentPos;
        currentPos += agent.velocity * ((ConnectionHandler.Instance.CurrentTimeMS - timestamp) / 1000.0f);
        transform.forward = new Vector3(xDir, 0 , yDir);
    }

    public void StartPath(float sx, float sy, float dx, float dy, float timestamp) {
        Ray ray = new Ray(new Vector3(dx, 100, dy), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            var hitPos = hit.point;
            var currentPos = transform.localPosition;
            currentPos.x = sx;
            currentPos.z = sy;
            transform.localPosition = currentPos;
            agent.SetDestination(hitPos);
            state = State.Moving;
        }
    }

    void OnGUI() {
        if (IsMine) {
            GUILayout.Button(this.Index.ToString());
            GUILayout.Button(agent.nextPosition.ToString());
            if (serverPath != null) {
                GUILayout.Button("path vertice count: " + serverPath.VerticesCount);
            }
        }
    }
}
