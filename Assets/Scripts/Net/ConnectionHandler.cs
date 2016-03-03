using UnityEngine;
using System.Collections;
using Google.ProtocolBuffers;
public class ConnectionHandler : MonoBehaviour
{
    public static ConnectionHandler Instance
    {
        get;
        private set;
    }
    private GameSocket socket;

    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
        if (socket != null)
            socket.Dispose();
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

    void OnReceiveMsg(byte[] bytes)
    {
        Debug.Log("receive bytes, length: " + bytes.Length);
    }
    public void Connect() {
        if (socket != null)
            socket.Dispose();
        StartCoroutine(StartConnect());
    }

    IEnumerator StartConnect()
    {
        socket = new GameSocket("127.0.0.1", 1234);
        while(!socket.Connected){
            yield return null;
        }
        socket.onReceiveMessage = this.OnReceiveMsg;
        StartCoroutine(socket.Dispatcher());
    }
    public void Send(System.ArraySegment<byte> bytes) {
        socket.Send(bytes);
    }

    public void Send(IMessage msg)
    {
        var stream = socket.GetStream();
        int size = msg.SerializedSize;
        unsafe {
            int* pSize = stackalloc int[1];
            pSize[0] = size;
            byte* bp = (byte*)pSize;
            stream.WriteByte(bp[3]);
            stream.WriteByte(bp[2]);
            stream.WriteByte(bp[1]);
            stream.WriteByte(bp[0]);
        }
        msg.WriteTo(stream);
        socket.Flush();
    }
}
