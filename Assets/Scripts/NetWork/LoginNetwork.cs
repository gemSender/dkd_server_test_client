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
        Player.Create(msg.Id, msg.X, msg.Y, false, msg.ColorIndex, msg.Timestamp);
    }
}
