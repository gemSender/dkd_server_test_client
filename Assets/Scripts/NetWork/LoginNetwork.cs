using UnityEngine;
using System.Collections;

public class LoginNetwork : GenNetwork<LoginNetwork> 
{
    protected override void Start()
    {
        base.Start();
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerLogin>(messages.PlayerLogin.ParseFrom, OtherPlayerLoginCallback);
    }

    void OtherPlayerLoginCallback(messages.PlayerLogin msg) {
        var go = Instantiate(Resources.Load("Player")) as GameObject;
        var goTrans = go.transform;
        goTrans.localPosition = new Vector3(msg.X, 0.5f, msg.Y);
        var player = goTrans.GetComponent<Player>();
        player.Id = msg.Id;
        player.IsMine = false;
    }
}
