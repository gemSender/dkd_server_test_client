using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.ProtocolBuffers;
using System.IO;
public class ConnectionHandler : MonoBehaviour
{
    public string host = "127.0.0.1";
    public int port = 1234;
    private Dictionary<string, System.Action<ByteString>> actionDict = new Dictionary<string, System.Action<ByteString>>();
    private Queue<System.Action<ByteString>> callbackQue = new Queue<System.Action<ByteString>>();
    private long loginTime;
    private float loginTimeSinceGameStartup;
    public long CurrentTimeMS {
        get {
            return loginTime + Mathf.FloorToInt((Time.realtimeSinceStartup - loginTimeSinceGameStartup) *  1000);
        }
    }

    public void SetLoginTime(long loginTime, float timeSecSinceLogin) {
        this.loginTime = loginTime;
        this.loginTimeSinceGameStartup = timeSecSinceLogin;
    }
    public static ConnectionHandler Instance
    {
        get;
        private set;
    }

    private GameSocket socket;

    public float TimeSinceLogin
    {
        get {
            return 0;
        }
    }
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else { 
            Instance = this; 
        }
        
    }

    public void RegisterCallBack<T>(System.Func<ByteString, T> parseFunc, System.Action<T> callBack)
    {
        actionDict[typeof(T).Name] = (bytes) =>
        {
            T msg = parseFunc(bytes);
            callBack(msg);
        };
    }

    void OnDestroy() {
        if (Instance == this)
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
        var msg = messages.GenReplyMsg.ParseFrom(bytes);
        //Debug.Log("receive message, type: " + msg.Type);
        if (msg.IsReply)
        {
            var action = callbackQue.Dequeue();
            action(msg.Data);
        }
        else
        {
            var action = actionDict[msg.Type];
            action(msg.Data);
        }
    }
    public void Connect(System.Action callback = null) {
        if (socket != null)
            socket.Dispose();
        StartCoroutine(StartConnect(callback));
    }

    IEnumerator StartConnect(System.Action callback = null)
    {
        socket = new GameSocket(host, port);
        socket.onReceiveMessage = this.OnReceiveMsg;
        socket.onDisconnect = this.OnDisconnected;
        socket.onConnected = this.OnConnected;
        socket.Connect();
        while(!socket.Connected){
            yield return null;
        }
        callback();
        StartCoroutine(socket.Dispatcher());
    }

    void OnConnected() {
        Debug.Log("connected", this);
    }

    void OnDisconnected() {
        Debug.Log("connection break");
    }

    void Send(byte[] array, int offset, int count) {
        var stream = socket.GetStream();
        if (stream.CanWrite) {
            stream.BeginWrite(array, offset, count, (s) => {
                stream.EndWrite(s); 
            }, null);
        }
    }

    public void SendUnpackedMessage(IMessage msg) {
        var builder = messages.GenMessage.CreateBuilder().SetData(msg.ToByteString()).SetType(msg.GetType().Name);
        Send(builder.Build());
    }

    public void SendUnpackedMessage<T>(IMessage msg, System.Func<ByteString, T> parserFunc, System.Action<T> callback) {
        System.Action<ByteString> callbackItem = (bs) =>
        {
            T reply = parserFunc(bs);
            callback(reply);
        };
        callbackQue.Enqueue(callbackItem);
        SendUnpackedMessage(msg);
    }

    void Send(IMessage msg)
    {
        int size = msg.SerializedSize;
        using(MemoryStream memStream = new MemoryStream(size + 4))
        {
            memStream.Write(System.BitConverter.GetBytes(size), 0, 4);
            msg.WriteTo(memStream);
            var bytes = memStream.GetBuffer();
            Send(bytes, 0, bytes.Length);
        }
    }
}
