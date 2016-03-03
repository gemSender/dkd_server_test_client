using UnityEngine;
using System.Collections;

public class ConnectionHandler : MonoBehaviour {
    public static ConnectionHandler instance
    {
        get;
        private set;
    }
    private GameSocket socket;

    void Awake() {
        instance = this;
    }

    void OnDestroy() {
        instance = null;
    }
    public bool Connectd {
        get {
            if (socket == null)
                return false;
            return socket.Connected;
        }
    }
    public bool ConnectionError
    {
        get {
            if (socket == null)
                return false;
            return socket.Error;
        }
    }
	// Use this for initialization

    public void Connect() {
        if (socket != null)
            socket.Dispose();
    }

    IEnumerator StartConnect()
    {
        socket = new GameSocket("127.0.0.1", 1234);
        while(!socket.Connected){
            yield return null;
        }
        StartCoroutine(socket.Dispatcher());
    }
    public void Send(object message) { 
        
    }
}
