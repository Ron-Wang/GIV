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
            readGiv = (GameObject)Instantiate(Resources.Load("Prefabs/ReadContour"));
            readGiv.GetComponent<ReadContour>().files = files;
        }
        else
        {
            readGiv = (GameObject)Instantiate(Resources.Load("Prefabs/ReadFem"));
            readGiv.GetComponent<ReadFem>().files = files;
        }
        readGiv.transform.parent = GetComponent<Transform>();

        /*
        if (File.Exists(file))
        {
            readTriangle = (GameObject)Instantiate(Resources.Load("Prefabs/ReadTBackground"));
            readTriangle.GetComponent<ReadTBackground>().file = file;
            readTriangle.transform.parent = GetComponent<Transform>();
        }
        file = fileDir + @"\line.givb";
        if (File.Exists(file))
        {
            readLine = (GameObject)Instantiate(Resources.Load("Prefabs/ReadLBackground"));
            readLine.GetComponent<ReadLBackground>().file = file;
            readLine.transform.parent = GetComponent<Transform>();
        }
        */

    }
}
