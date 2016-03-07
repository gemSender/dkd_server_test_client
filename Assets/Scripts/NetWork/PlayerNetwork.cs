using UnityEngine;
using System.Collections;

public class PlayerNetwork : GenNetwork<PlayerNetwork> {
    protected override void Start()
    {
        base.Start();
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerMoveTo>(messages.PlayerMoveTo.ParseFrom, OnPlayerMoveTo);
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerQuit>(messages.PlayerQuit.ParseFrom, OnPlayerQuit);
        ConnectionHandler.Instance.RegisterCallBack<messages.PlayerStartPath>(messages.PlayerStartPath.ParseFrom, OnPlayerStartPath);
    }

    void OnPlayerMoveTo(messages.PlayerMoveTo msg) {
        var player = Player.GetPlayer(msg.Id);
        player.MoveTo(msg.X, msg.Y, msg.DirX, msg.DirY, msg.Timestamp);
    }

    void OnPlayerQuit(messages.PlayerQuit msg) {
        Player.RemovePlayer(msg.Id);
    }

    void OnPlayerStartPath(messages.PlayerStartPath msg) {
        Player.GetPlayer(msg.Id).StartPath(msg.Sx, msg.Sy, msg.Dx, msg.Dy, msg.Timestamp);
    }
}
