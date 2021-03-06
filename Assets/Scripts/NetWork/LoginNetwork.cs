﻿using UnityEngine;
using System.Collections;

public class LoginNetwork : GenNetwork<LoginNetwork> 
{
    protected override void Start()
    {
        base.Start();
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerLogin>(messages.PlayerLogin.ParseFrom, OtherPlayerLoginCallback);
    }

    void OtherPlayerLoginCallback(messages.PlayerLogin msg) {
        var playerState = msg.PlayerData;
        Player.Create(playerState.Index, playerState.X, playerState.Y, false, playerState.ColorIndex, msg.Timestamp);
    }
}
