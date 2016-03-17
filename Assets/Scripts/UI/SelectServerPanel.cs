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

    public void ShowNavMesh()
    {
        var trianInfo = NavMesh.CalculateTriangulation();
        GameObject go = new GameObject("navmesh", typeof(MeshFilter), typeof(MeshRenderer));
        var mf = go.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.vertices = trianInfo.vertices;
        mf.mesh.triangles = trianInfo.indices;
        mf.mesh.RecalculateNormals();
        Debug.Log("verticesCount: " + mf.mesh.vertexCount);
    }
}
