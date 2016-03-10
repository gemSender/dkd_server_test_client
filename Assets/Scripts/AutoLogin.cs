using UnityEngine;
using System.Collections;
using messages;
using Google.ProtocolBuffers;

public class AutoLogin : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        //ConnectionHandler.Instance.Connect();
        while (!ConnectionHandler.Instance.Connectd) {
            yield return null;
        }
        var equipId = SystemInfo.deviceUniqueIdentifier;
        float time1 = Time.realtimeSinceStartup;
        ConnectionHandler.Instance.SendUnpackedMessage<LoginReply>(Login.CreateBuilder().SetEquipId(equipId).Build(),
            LoginReply.ParseFrom,
            (reply) => {
                if (reply.ErrorCode == 0)
                {
                    float time2 = Time.realtimeSinceStartup;
                    ConnectionHandler.Instance.SetLoginTime(reply.Timestamp, (time2 - time1) / 2);
                    var myState = reply.MyState;
                    Player.Create(myState.Index, myState.X, myState.Y, true, myState.ColorIndex, reply.Timestamp);
                    foreach (var item in reply.PlayersList)
                    {
                        Player.Create(item.Index, item.X, item.Y, false, item.ColorIndex, reply.Timestamp);
                    }
                }
                else { 
                
                }
            });
	}
}
