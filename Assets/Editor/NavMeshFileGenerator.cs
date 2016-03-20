using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class NavMeshFileGenerator {

    [MenuItem("Window/GenerateNavmeshFile")]
    public static void GenerateFile() {
        Dictionary<Vector2, int> vertDict = new Dictionary<Vector2, int>();
        try
        {
            using (FileStream fs = File.OpenWrite(Path.Combine(Application.dataPath, "NavMesh/navmesh.bytes")))
            {
                var mesh = NavMesh.CalculateTriangulation();
                var vertices = mesh.vertices;
                for (int i = 0, j = 0; i < vertices.Length; i++) { 
                    var p = vertices[i];
                    var p2d = new Vector2(p.x, p.z);
                    int index = 0;
                    if (!vertDict.TryGetValue(p2d, out index)) {
                        vertDict[p2d] = j;
                        j++;
                    }
                }
                List<KeyValuePair<Vector2, int>> vertList = vertDict.ToList();
                vertList.Sort((a, b) => a.Value - b.Value);
                var vertLenBytes = BitConverter.GetBytes(vertList.Count);
                fs.Write(vertLenBytes, 0, vertLenBytes.Length);
                for (int i = 0; i < vertList.Count; i++)
                {
                    var p = vertList[i].Key;
                    var b1 = BitConverter.GetBytes(p.x);
                    var b2 = BitConverter.GetBytes(p.y);
                    fs.Write(b1, 0, b1.Length);
                    fs.Write(b2, 0, b2.Length);
                }
                Debug.LogFormat("vert count reduce from {0} to {1}", vertices.Length, vertList.Count);
                var triIndices = mesh.indices;
                var triIndLenBytes = BitConverter.GetBytes(triIndices.Length);
                var newIndice = new int[triIndices.Length];
                fs.Write(triIndLenBytes, 0, triIndLenBytes.Length);
                for (int i = 0; i < triIndices.Length; i += 3)
                {
                    var p1 = vertices[triIndices[i]];
                    var p2 = vertices[triIndices[i + 1]];
                    var p3 = vertices[triIndices[i + 2]];
                    var idx1 = vertDict[new Vector2(p1.x, p1.z)];
                    var idx2 = vertDict[new Vector2(p2.x, p2.z)];
                    var idx3 = vertDict[new Vector2(p3.x, p3.z)];
                    newIndice[i] = idx1;
                    newIndice[i + 1] = idx2;
                    newIndice[i + 2] = idx3;
                    var idx1Bytes = BitConverter.GetBytes(idx1);
                    var idx2Bytes = BitConverter.GetBytes(idx2);
                    var idx3Bytes = BitConverter.GetBytes(idx3);
                    fs.Write(idx1Bytes, 0, idx1Bytes.Length);
                    fs.Write(idx2Bytes, 0, idx2Bytes.Length);
                    fs.Write(idx3Bytes, 0, idx3Bytes.Length);
                }
                var areaIndeices = mesh.areas;
                var areaIndLenBytes = BitConverter.GetBytes(areaIndeices.Length);
                fs.Write(areaIndLenBytes, 0, areaIndLenBytes.Length);
                for (int i = 0; i < areaIndeices.Length; i++) {
                    var area = areaIndeices[i];
                    var areaBytes = BitConverter.GetBytes(area);
                    fs.Write(areaBytes, 0, areaBytes.Length);
                }
                Debug.LogFormat("triLen: {0}, areaIndLen: {1}", triIndices.Length, areaIndeices.Length);
                RenderNavMesh(vertList.Select((x) => new Vector3(x.Key.x, 0, x.Key.y)).ToArray(), newIndice);
            }
            AssetDatabase.Refresh();
        }
        catch(Exception e)
        {
            Debug.Log(e.StackTrace);
        }   
    }

    static void RenderNavMesh(Vector3[] vertices, int[] triangles){
        GameObject go = GameObject.Find("NavmeshRender");
        var mf = go.GetComponent<MeshFilter>();
        GameObject.DestroyImmediate(mf.sharedMesh);
        var mesh = mf.sharedMesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
