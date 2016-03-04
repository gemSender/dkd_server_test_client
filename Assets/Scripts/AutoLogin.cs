using UnityEngine;
using System.Collections;
using messages;
using Google.ProtocolBuffers;

public class AutoLogin : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        ConnectionHandler.Instance.Connect();
        while (!ConnectionHandler.Instance.Connectd) {
            yield return null;
        }
        ConnectionHandler.Instance.SendUnpackedMessage(Login.CreateBuilder().SetId("asdfasdf").Build());
	}
}
