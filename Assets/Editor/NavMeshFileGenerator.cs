using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class NavMeshFileGenerator {

    [MenuItem("Window/GenerateNavmeshFile")]
    public static void GenerateFile() {
        try
        {
            using (FileStream fs = File.OpenWrite(Path.Combine(Application.dataPath, "NavMesh/navmesh.bytes")))
            {
                var mesh = NavMesh.CalculateTriangulation();
                var vertices = mesh.vertices;
                var vertLenBytes = BitConverter.GetBytes(vertices.Length);
                fs.Write(vertLenBytes, 0, vertLenBytes.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    var p = vertices[i];
                    var b1 = BitConverter.GetBytes(p.x);
                    var b2 = BitConverter.GetBytes(p.z);
                    fs.Write(b1, 0, b1.Length);
                    fs.Write(b2, 0, b2.Length);
                }
                var triIndices = mesh.indices;
                var triIndLenBytes = BitConverter.GetBytes(triIndices.Length);
                fs.Write(triIndLenBytes, 0, triIndLenBytes.Length);
                for (int i = 0; i < triIndices.Length; i += 3)
                {
                    var idx1 = triIndices[i];
                    var idx2 = triIndices[i + 1];
                    var idx3 = triIndices[i + 2];
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
            }
            AssetDatabase.Refresh();
        }
        catch(Exception e)
        {
            Debug.Log(e.StackTrace);
        }   
    }
}
