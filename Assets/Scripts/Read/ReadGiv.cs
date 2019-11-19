using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.IO;

public class ReadGiv : MonoBehaviour {
    //数据文件路径
    public string[] files;
    bool changeMesh;
    GameObject readGiv,readTriangle,readLine;
    long jf;

    void Start () {
        jf = 0;
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[0]))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                accessor.Read<bool>(jf, out changeMesh);
            }
        }
        if (changeMesh)
        {
            readGiv = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/ReadContour"));
            readGiv.GetComponent<ReadContour>().files = files;
        }
        else
        {
            readGiv = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/ReadFem"));
            readGiv.GetComponent<ReadFem>().files = files;
        }
        readGiv.transform.parent = GetComponent<Transform>();

    }
}
