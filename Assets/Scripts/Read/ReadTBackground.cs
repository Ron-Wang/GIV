using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

public class ReadTBackground : MonoBehaviour {
    public string file;
    int meshNum;
    int[] meshVNum;
    int[] meshTNum;
    float[] value;
    GameObject[] pre;

    // Use this for initialization
    void Start () {
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeF = sizeof(float);
        long jf = 0;

        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(file))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                accessor.Read<int>(jf, out meshNum);
                jf += sizeI;
                pre = new GameObject[meshNum];
                meshVNum = new int[meshNum];
                meshTNum = new int[meshNum];
                value = new float[meshNum];
                for (int j = 0; j < meshNum; j++)
                {
                    accessor.Read<int>(jf, out meshVNum[j]);
                    jf += sizeI;
                    Vector3[] vertices = new Vector3[meshVNum[j]];
                    accessor.Read<int>(jf, out meshTNum[j]);
                    jf += sizeI;
                    int[] triangles = new int[meshTNum[j]];
                    accessor.ReadArray<Vector3>(jf, vertices, 0, meshVNum[j]);
                    jf += sizeV * meshVNum[j];
                    accessor.ReadArray<int>(jf, triangles, 0, meshTNum[j]);
                    jf += sizeI * meshTNum[j];
                    accessor.Read<float>(jf, out value[j]);
                    jf += sizeF;
                    //创建预设体，赋予顶点和拓扑 
                    pre[j] = (GameObject)Instantiate(Resources.Load("Prefabs/PreTriangle"));
                    pre[j].transform.parent = GetComponent<Transform>();
                    Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                    mesh.Clear();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();
                }
            }
        }
    }
}
