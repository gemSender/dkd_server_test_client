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
        var id = System.Guid.NewGuid().ToString("N");
        float time1 = Time.realtimeSinceStartup;
        ConnectionHandler.Instance.SendUnpackedMessage<LoginReply>(Login.CreateBuilder().SetId(id).Build(),
            LoginReply.ParseFrom,
            (reply) => {
                float time2 = Time.realtimeSinceStartup;
                ConnectionHandler.Instance.SetLoginTime(reply.Timestamp, (time2 - time1) / 2);
                Player.Create(id, reply.X, reply.Y, true, reply.ColorIndex, reply.Timestamp);
                foreach (var item in reply.PlayersList) {
                    Player.Create(item.Id, item.X, item.Y, false, item.ColorIndex, reply.Timestamp);
                }
            });
	}
}
