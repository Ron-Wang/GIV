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
    int timeNow = -1;

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
        menuItem1 = (GameObject)Instantiate(Resources.Load("Prefabs/MenuItem1"));
        menuItem1.GetComponent<MenuItem1>().timeSum = timeNum;
        objects = GameObject.Find("Objects");
        pre[0] = (GameObject)Instantiate(Resources.Load("Prefabs/PreContour"),objects.GetComponent<Transform>().position,
                                objects.GetComponent<Transform>().rotation);
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
                            pre[k] = (GameObject)Instantiate(Resources.Load("Prefabs/PreContour"), 
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
        }
    }

}
