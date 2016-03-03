using UnityEngine;
using System.Collections;
using messages;
using Google.ProtocolBuffers;

public class AutoLogin : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        ConnectionHandler.Instance.Connect();
        while (!ConnectionHandler.Instance.Connectd) {
            Debug.Log("fix bug");
            yield return null;
        }
        var builder = Login.CreateBuilder();
        builder.SetId("fuck proto gen");
        var msg = builder.Build();
        var genMsgBuilder = GemMessage.CreateBuilder();
        genMsgBuilder.SetType(msg.GetType().Name);
        genMsgBuilder.SetData(msg.ToByteString());
        
        ConnectionHandler.Instance.Send(genMsgBuilder.Build());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
