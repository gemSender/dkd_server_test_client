using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectServerPanel : MonoBehaviour {
    public InputField input;
    public void OnConnectBtn() {
        string address = input.text;
        string[] ip_port = address.Split(':');
        if (ip_port.Length == 2) {
            var ip = ip_port[0];
            int port = 1234;
            if (int.TryParse(ip_port[1], out port)) {
                ConnectionHandler.Instance.host = ip;
                ConnectionHandler.Instance.port = port;
                ConnectionHandler.Instance.Connect(this.Hide);
            }
        }
    }

    public void Hide() {
        Destroy(gameObject);
    }
}
