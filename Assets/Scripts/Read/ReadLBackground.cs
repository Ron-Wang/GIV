using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO;

public class ReadLBackground : MonoBehaviour
{
    public string file;
    int lineVNum;
    GameObject[] pre;

    // Use this for initialization
    void Start()
    {
        long sizeI = sizeof(int);
        long jf = 0;
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(file))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                accessor.Read<int>(jf, out lineVNum);
                pre = new GameObject[lineVNum];
                jf += sizeI;
                Vector3[] vertices = new Vector3[2 * lineVNum];
                accessor.ReadArray<Vector3>(jf, vertices, 0, 2 * lineVNum);
                for (int j = 0; j < lineVNum; j++)
                {
                    //创建预设体，赋予顶点和拓扑 
                    pre[j] = (GameObject)Instantiate(Resources.Load("Prefabs/PreLine"));
                    pre[j].transform.parent = GetComponent<Transform>();
                    pre[j].GetComponent<LineRenderer>().SetPosition(0, vertices[2 * j]);
                    pre[j].GetComponent<LineRenderer>().SetPosition(1, vertices[2 * j + 1]);
                }
            }
        }
    }
}
