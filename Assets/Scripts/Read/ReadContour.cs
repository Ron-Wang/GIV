using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class ReadContour : MonoBehaviour {
    public string[] files;
    int timeNum;
    int meshNum,meshNumOld = 1;
    int[] meshVNum;
    int[] meshTNum;
    float[] value;
    GameObject objects;
    GameObject[] pre = new GameObject[1];
    GameObject[][] BgPre;
    int[] BgNum;
    int timeNow = -1;
    float ttt = 0;/// <summary>
                  /// ce shi yong
                  /// </summary>

    GameObject menuItem1;
    private GameObject panelMenu,panelColor,panelArraw;

    Color[] colors = new Color[] { Color.white, Color.blue, Color.red, Color.yellow, Color.green };

    // Use this for initialization
    void Start () {
        panelMenu = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu");
        panelMenu.SetActive(false);
        panelColor = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelColor");
        panelColor.SetActive(false);
        panelArraw = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelArraw");
        panelArraw.SetActive(false);
        timeNum = files.Length;
        menuItem1 = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/MenuItem1"));
        menuItem1.GetComponent<MenuItem1>().timeSum = timeNum;
        objects = GameObject.Find("Objects");
        pre[0] = (GameObject)Instantiate(Resources.Load("Prefabs/Pres/PreContour"),objects.GetComponent<Transform>().position,
                                objects.GetComponent<Transform>().rotation);
        string dir = Path.GetDirectoryName(files[0]);
        string[] BgPath = Directory.GetFiles(dir, "*.givb");
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeF = sizeof(float);
        float[] value_;
        if (BgPath.Length > 0)
        {
            BgPre = new GameObject[BgPath.Length][];
            BgNum = new int[BgPath.Length];
            Color[] cBg = new Color[] 
            { new Color(0.5f,0.5f,0.5f,0.5f), new Color(1,0,0,0.5f),new Color(0,1,0,0.5f)
            , new Color(0,0,1,0.5f), new Color(1,0.92f,0.016f,0.5f) };
            Color cBgNow;
            for (int i = 0; i < BgPath.Length; ++i)
            {
                long jf = 0;
                cBgNow = cBg[i % 5];
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(BgPath[i]))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        accessor.Read<int>(jf, out meshNum);
                        jf += sizeI;
                        BgPre[i] = new GameObject[meshNum];
                        BgNum[i] = meshNum;
                        meshVNum = new int[meshNum];
                        meshTNum = new int[meshNum];
                        value_ = new float[meshNum];
                        for (int k = 0; k < meshNum; k++)
                        {
                            BgPre[i][k] = (GameObject)Instantiate(Resources.Load("Prefabs/Pres/PreTriangle"),
                                objects.GetComponent<Transform>().position,
                                objects.GetComponent<Transform>().rotation);
                            BgPre[i][k].GetComponent<MeshRenderer>().materials[0].color = cBgNow;
                            //BgPre[i][k].GetComponent<MeshRenderer>().materials[0].color = new Color(1, 1, 1, 0);
                        }
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
                            accessor.Read<float>(jf, out value_[j]);
                            jf += sizeF;
                            //创建预设体，赋予顶点和拓扑 
                            BgPre[i][j].transform.parent = GetComponent<Transform>();
                            Mesh mesh = BgPre[i][j].GetComponent<MeshFilter>().mesh;
                            mesh.Clear();
                            mesh.vertices = vertices;
                            mesh.triangles = triangles;
                            mesh.RecalculateNormals();
                        }
                    }
                }
            }
        }
    }


	// Update is called once per frame
	void Update () {
        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeF = sizeof(float);

        long jf = sizeB;

        //Is Color
        if (menuItem1.GetComponent<MenuItem1>().isColorChange)
        {
            for (int j = 0; j < meshNumOld; j++)
                pre[j].GetComponent<MeshRenderer>().materials[1].color = colors[menuItem1.GetComponent<MenuItem1>().colorIndex];
        }

        //Is Mesh
        if (menuItem1.GetComponent<MenuItem1>().isMeshOnChange)
        {
            if (menuItem1.GetComponent<MenuItem1>().isMesh)
                for (int j = 0; j < meshNumOld; j++)
                    pre[j].GetComponent<MeshRenderer>().materials[0].color = Color.black;
            else
                for (int j = 0; j < meshNumOld; j++)
                    pre[j].GetComponent<MeshRenderer>().materials[0].color = new Color(1, 1, 1, 0);
        }
        //Is Bg
        if (menuItem1.GetComponent<MenuItem1>().isBgOnChange)
        {
            if (menuItem1.GetComponent<MenuItem1>().isBg)
                for (int j = 0; j < BgNum.Length; j++)
                    for (int k = 0; k < BgNum[j]; ++k)
                        BgPre[j][k].SetActive(true);
            else
                for (int j = 0; j < BgNum.Length; j++)
                    for (int k = 0; k < BgNum[j]; ++k)
                        BgPre[j][k].SetActive(false);
        }

        if (menuItem1.GetComponent<MenuItem1>().timeNow != timeNow)
        {
            timeNow = menuItem1.GetComponent<MenuItem1>().timeNow;
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[timeNow]))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Read<int>(jf, out meshNum);
                    jf += sizeI + sizeV * 2;//maxVertice and minVertice
                    meshVNum = new int[meshNum];
                    meshTNum = new int[meshNum];
                    value = new float[meshNum];
                    if (meshNumOld < meshNum)
                    {
                        Array.Resize(ref pre, meshNum);
                        for (int k = meshNumOld; k < meshNum; k++)
                        {
                            pre[k] = (GameObject)Instantiate(Resources.Load("Prefabs/Pres/PreContour"), 
                                objects.GetComponent<Transform>().position,
                                objects.GetComponent<Transform>().rotation);
                            pre[k].GetComponent<MeshRenderer>().materials[1].color = colors[menuItem1.GetComponent<MenuItem1>().colorIndex];
                            if (menuItem1.GetComponent<MenuItem1>().isMesh)
                                pre[k].GetComponent<MeshRenderer>().materials[0].color = Color.black;
                            else
                                pre[k].GetComponent<MeshRenderer>().materials[0].color = new Color(1, 1, 1, 0);
                        }
                    }
                    else if (meshNumOld > meshNum)
                    {
                        for (int k = meshNum; k < meshNumOld; k++)
                        {
                            Destroy(pre[k]);
                        }
                    }
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
                        pre[j].transform.parent = GetComponent<Transform>();
                        Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                        mesh.Clear();
                        mesh.vertices = vertices;
                        mesh.triangles = triangles;
                        mesh.RecalculateNormals();
                    }
                }
            }
            meshNumOld = meshNum;
            if (timeNow == 0)
            {
                Debug.Log(Time.time - ttt);////////////////////////////
                ttt = Time.time;

            }
        }
    }

}
