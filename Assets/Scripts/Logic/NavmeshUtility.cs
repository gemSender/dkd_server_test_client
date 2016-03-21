using UnityEngine;
using System.Collections;
using System.IO;

public class NavmeshUtility : MonoBehaviour {
    public static Vector2[] vertices;
    public static int[] triangle;
	// Use this for initialization
	void Start () {
        using (var fs = File.OpenRead(Path.Combine(Application.dataPath, "NavMesh/navmesh.bytes")))
        {
            var lenBuf = new byte[4];
            fs.Read(lenBuf, 0, 4);
            int vertLen;
            unsafe
            {
                fixed (void* vp = lenBuf)
                {
                    vertLen = *((int*)vp);
                }
            }
            Debug.Log("vertLen:" + vertLen);
            vertices = new Vector2[vertLen];
            unsafe
            {
                byte[] vecBuf = new byte[2 * sizeof(float)];
                fixed (Vector2* pv = vertices)
                {
                    for (int i = 0; i < vertLen; i++)
                    {
                        fs.Read(vecBuf, 0, vecBuf.Length);
                        fixed (void* vb = vecBuf)
                        {
                            pv[i] = *((Vector2*)vb);
                        }
                    }
                }
            }
            int triLen;
            fs.Read(lenBuf, 0, 4);
            unsafe
            {
                fixed (void* vp = lenBuf)
                {
                    triLen = *((int*)vp);
                }
            }
            Debug.Log("triLen:" + triLen);
            triangle = new int[triLen];
            unsafe
            {
                byte[] intBuf = new byte[sizeof(int)];
                for (int i = 0; i < triLen; i++)
                {
                    fs.Read(intBuf, 0, intBuf.Length);
                    fixed (void* vb = intBuf)
                    {
                        triangle[i] = *((int*)vb);
                    }
                }
            }
        }
        var parent = new GameObject("Text").transform;
        parent.transform.position = GameObject.Find("NavmeshRender").transform.position;
        for (int i = 0; i < vertices.Length; i++) {
            var txt = Instantiate(Resources.Load("VertIndex")) as GameObject;
            txt.GetComponent<TextMesh>().text = i.ToString();
            txt.transform.parent = parent;
            txt.name = i.ToString();
            txt.transform.localPosition = new Vector3(vertices[i].x, 0, vertices[i].y);
        }
            for (int i = 0; i < triangle.Length; i += 3)
            {
                int triIdx = i / 3;
                Vector2 A = vertices[triangle[i]];
                Vector2 B = vertices[triangle[i + 1]];
                Vector2 C = vertices[triangle[i + 2]];
                var center = (A + B + C) / 3;
                var txt = Instantiate(Resources.Load("TriangleIndex")) as GameObject;
                txt.GetComponent<TextMesh>().text = triIdx.ToString();
                txt.transform.parent = parent;
                txt.name = triIdx.ToString();
                txt.transform.localPosition = new Vector3(center.x, 0, center.y);
            }
	}

    public static int GetTriangleIndex(Vector2 P){
        for (int i = 0; i < triangle.Length; i += 3) {
            Vector2 A = vertices[triangle[i]];
            Vector2 B = vertices[triangle[i + 1]];
            Vector2 C = vertices[triangle[i + 2]];
            if (PointinTriangle1(A, B, C, P)) {
                return i / 3;
            }
        }
        return -1;
    }

    public static bool PBetweenABAC(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        var AP = P - A;
        var PB = B - P;
        var PC = C - P;
        Vector3 v1 = Vector3.Cross(AP, PB);
        Vector3 v2 = Vector3.Cross(AP, PC);
        return v1.z * v2.z < 0;
    }

    public static bool PointinTriangle1(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        return PBetweenABAC(A, B, C, P) &&
            PBetweenABAC(B, C, A, P) &&
            PBetweenABAC(C, A, B, P);
    }
}
