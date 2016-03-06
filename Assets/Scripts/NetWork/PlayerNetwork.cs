using UnityEngine;
using System.Collections;

public class PlayerNetwork : GenNetwork<PlayerNetwork> {
    protected override void Start()
    {
        base.Start();
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerMoveTo>(messages.PlayerMoveTo.ParseFrom, OnPlayerMoveTo);
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerQuit>(messages.PlayerQuit.ParseFrom, OnPlayerQuit);
    }

    void OnPlayerMoveTo(messages.PlayerMoveTo msg) {
        var player = Player.GetPlayer(msg.Id);
        player.MoveTo(msg.X, msg.Y, msg.DirX, msg.DirY, msg.Timestamp);
    }

    void OnPlayerQuit(messages.PlayerQuit msg) {
        Player.RemovePlayer(msg.Id);
    }
}
