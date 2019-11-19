using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

public class ReadFem : MonoBehaviour {
    public string[] files;

    int timeNum;
    int meshNum;
    int scalarNum;
    int vectorNum;
    bool isVerAdd;
    int areaNum;
    float ttt = 0;/// <summary>
    /// ce shi yong
    /// </summary>

    Vector3 maxVertice, minVectice;

    int[] meshVNum;
    int[] meshTNum;
    Vector3[][] vertices;


    Color[][] colors;
    Quaternion[][] quats;
    float[][] values;
    Vector3[][] Avertices;

    GameObject objects;
    GameObject[] pre;
    int charLength = 30;
    long[] sizeMesh, sizeScalar, sizeVector,sizeVerAdd;
    int timeNow = 0;
    int scalarNow = 0;
    int vectorNow = 0;
    GameObject arraw;
    GameObject[][] preArraw;

    GameObject menuItem;
    private GameObject panelMenu1;
    private GameObject panelColor, panelArraw;
    private Dropdown dArea,dScalar, dVector;
    private Text tMax, tMin,tMax1,tMin1;

    private int areaNow = 0;
    private long areaStart = 0;

    // Use this for initialization
    void Start() {
        panelMenu1 = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu1");
        panelMenu1.SetActive(false);
        panelColor = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelColor");
        panelArraw = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelArraw");
        timeNum = files.Length;
        menuItem = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/MenuItem"));
        menuItem.GetComponent<MenuItem>().timeSum = timeNum;
        objects = GameObject.Find("Objects");
        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeChar = sizeof(char);
        long sizeF = sizeof(float);
        long sizeC = Marshal.SizeOf(typeof(Color));
        long sizeQ = Marshal.SizeOf(typeof(Quaternion));
        long jf = sizeB;
        string dir = Path.GetDirectoryName(files[0]);

        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[0]))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                accessor.Read<int>(jf, out meshNum);
                jf += sizeI;
                accessor.Read<int>(jf, out scalarNum);
                jf += sizeI;
                accessor.Read<int>(jf, out vectorNum);
                jf += sizeI;
                accessor.Read<bool>(jf, out isVerAdd);
                jf += sizeB;
                accessor.Read<int>(jf, out areaNum);
                jf += sizeI;
                sizeMesh = new long[areaNum];
                sizeScalar = new long[areaNum];
                sizeVector = new long[areaNum];
                sizeVerAdd = new long[areaNum];
                Dropdown.OptionData optionData1;
                dArea = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/DropdownArea").GetComponent<Dropdown>();
                dArea.options.Clear();
                for (int i = 0; i < areaNum; ++i)
                {
                    char[] areaName = new char[charLength];
                    accessor.ReadArray<char>(jf, areaName, 0, charLength);
                    optionData1 = new Dropdown.OptionData();
                    optionData1.text = new string(areaName);
                    dArea.options.Add(optionData1);
                    jf += sizeChar * charLength;
                }
                dArea.captionText.text = dArea.options[0].text;

                pre = new GameObject[meshNum];
                vertices = new Vector3[meshNum][];
                Avertices = new Vector3[meshNum][];
                colors = new Color[meshNum][];
                jf += sizeV * 2;//maxVertice and minVertice
                meshVNum = new int[meshNum];
                meshTNum = new int[meshNum];
                sizeScalar[0] = sizeChar * charLength + 2 * sizeF;
                sizeVector[0] = sizeChar * charLength + 2 * sizeF;
                sizeVerAdd[0] = 0;
                for (int j = 0; j < meshNum; j++)
                {
                    accessor.Read<int>(jf, out meshVNum[j]);
                    jf += sizeI;
                    accessor.Read<int>(jf, out meshTNum[j]);
                    jf += sizeI;
                    if (meshVNum[j] != 0)
                    {
                        vertices[j] = new Vector3[meshVNum[j]];
                        int[] triangles = new int[meshTNum[j]];
                        colors[j] = new Color[meshVNum[j]];
                        accessor.ReadArray<Vector3>(jf, vertices[j], 0, meshVNum[j]);
                        jf += sizeV * meshVNum[j];
                        accessor.ReadArray<int>(jf, triangles, 0, meshTNum[j]);
                        jf += sizeI * meshTNum[j];
                        //创建预设体，赋予顶点和拓扑 
                        pre[j] = (GameObject)Instantiate(Resources.Load("Prefabs/Pres/PreFem"), 
                            objects.GetComponent<Transform>().position,
                                objects.GetComponent<Transform>().rotation);
                        pre[j].transform.parent = GetComponent<Transform>();
                        Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                        mesh.Clear();
                        mesh.vertices = vertices[j];
                        mesh.triangles = triangles;
                        mesh.RecalculateNormals();
                        sizeScalar[0] += sizeC * meshVNum[j];
                        sizeVector[0] += (sizeQ + sizeF) * meshVNum[j];
                        if (isVerAdd)
                            sizeVerAdd[0] += sizeV * meshVNum[j];
                    }
                }
                sizeMesh[0] = jf;

                //读入标量矢量信息，变形与否
                Dropdown.OptionData optionData;
                dScalar = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleSca/DropdownMesh").GetComponent<Dropdown>();
                dScalar.options.Clear();
                for (int i = 0; i < scalarNum; ++i)
                {
                    jf = sizeMesh[0] + sizeScalar[0] * i;
                    char[] infoName = new char[charLength];
                    accessor.ReadArray<char>(jf, infoName, 0, charLength);
                    optionData = new Dropdown.OptionData();
                    optionData.text = new string(infoName);
                    dScalar.options.Add(optionData);
                }
                dScalar.captionText.text = dScalar.options[0].text;
                dVector = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleVec/DropdownMesh").GetComponent<Dropdown>();
                dVector.options.Clear();
                for (int i = 0; i < vectorNum; ++i)
                {
                    jf = sizeMesh[0] + sizeScalar[0] * scalarNum + sizeVector[0] * i;
                    char[] infoName = new char[charLength];
                    accessor.ReadArray<char>(jf, infoName, 0, charLength);
                    optionData = new Dropdown.OptionData();
                    optionData.text = new string(infoName);
                    dVector.options.Add(optionData);
                }
                dVector.captionText.text = dVector.options[0].text;
                jf = sizeMesh[0] + sizeScalar[0] * scalarNum + sizeVector[0] * vectorNum + sizeVerAdd[0];
                for (int areaIndex = 1; areaIndex < areaNum; ++areaIndex)
                {
                    sizeScalar[areaIndex] = sizeChar * charLength + 2 * sizeF;
                    sizeVector[areaIndex] = sizeChar * charLength + 2 * sizeF;
                    sizeVerAdd[areaIndex] = 0;
                    jf += sizeV * 2;
                    int[] meshVNumS = new int[meshNum];
                    int[] meshTNumS = new int[meshNum];
                    for (int j = 0; j < meshNum; j++)
                    {
                        accessor.Read<int>(jf, out meshVNumS[j]);
                        jf += sizeI;
                        accessor.Read<int>(jf, out meshTNumS[j]);
                        jf += sizeI;
                        jf += sizeV * meshVNumS[j] + sizeI * meshTNumS[j];
                        sizeScalar[areaIndex] += sizeC * meshVNumS[j];
                        sizeVector[areaIndex] += (sizeQ + sizeF) * meshVNumS[j];
                        if (isVerAdd)
                            sizeVerAdd[areaIndex] += sizeV * meshVNumS[j];
                    }
                    sizeMesh[areaIndex] = jf;
                    jf = sizeMesh[areaIndex] + sizeScalar[areaIndex] * scalarNum 
                        + sizeVector[areaIndex] * vectorNum + sizeVerAdd[areaIndex];
                }
            }
        }

        menuItem.GetComponent<MenuItem>().isVerAdd = isVerAdd;
        tMax = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelColor/TextMax").GetComponent<Text>();
        tMin = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelColor/TextMin").GetComponent<Text>();
        arraw = Resources.Load("Prefabs/Arraw") as GameObject;
        tMax1 = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelArraw/TextMax").GetComponent<Text>();
        tMin1 = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelArraw/TextMin").GetComponent<Text>();

        panelArraw.SetActive(false);
    }


    // Update is called once per frame
    void Update() {
        //Is Area
        if (menuItem.GetComponent<MenuItem>().isAreaChange)
        {
            long sizeB = Marshal.SizeOf(typeof(bool));
            long sizeI = sizeof(int);
            long sizeV = Marshal.SizeOf(typeof(Vector3));
            long sizeChar = sizeof(char);
            areaNow = menuItem.GetComponent<MenuItem>().areaIndex;
            if (areaNow == 0)
                areaStart = sizeB * 2 + sizeI * 4 + sizeChar * charLength * areaNum;
            else
                areaStart = sizeMesh[areaNow - 1] + sizeScalar[areaNow - 1] * scalarNum
                    + sizeVector[areaNow - 1] * vectorNum + sizeVerAdd[areaNow - 1];
            long jf = areaStart + sizeV * 2;
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[0]))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    for (int j = 0; j < meshNum; ++j)
                    {
                        accessor.Read<int>(jf, out meshVNum[j]);
                        jf += sizeI;
                        accessor.Read<int>(jf, out meshTNum[j]);
                        jf += sizeI;
                        Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                        mesh.Clear();
                        if (meshVNum[j] != 0)
                        {
                            vertices[j] = new Vector3[meshVNum[j]];
                            int[] triangles = new int[meshTNum[j]];
                            colors[j] = new Color[meshVNum[j]];
                            accessor.ReadArray<Vector3>(jf, vertices[j], 0, meshVNum[j]);
                            jf += sizeV * meshVNum[j];
                            accessor.ReadArray<int>(jf, triangles, 0, meshTNum[j]);
                            jf += sizeI * meshTNum[j];
                            mesh.vertices = vertices[j];
                            mesh.triangles = triangles;
                            mesh.RecalculateNormals();
                        }
                    }
                }
            }
            if (menuItem.GetComponent<MenuItem>().isMesh)
            {
                for (int j = 0; j < meshNum; j++)
                    if (meshVNum[j] != 0)
                        pre[j].GetComponent<MeshRenderer>().materials[0].color = Color.black;
            }
            else
            {
                for (int j = 0; j < meshNum; j++)
                    if (meshVNum[j] != 0)
                        pre[j].GetComponent<MeshRenderer>().materials[0].color = new Color(1, 1, 1, 0);
            }
            if (menuItem.GetComponent<MenuItem>().isScalar)
                ScalarDisplay();
            if (menuItem.GetComponent<MenuItem>().isVector)
            {
                VectorDestroy();
                VectorCreate();
                VectorDisplay();
            }
            if (menuItem.GetComponent<MenuItem>().isScale)
            {

                DeformationDisplay();
            }
        }

        //Is Mesh
        if (menuItem.GetComponent<MenuItem>().isMeshOnChange)
        {
            if (menuItem.GetComponent<MenuItem>().isMesh)
            {
                for (int j = 0; j < meshNum; j++)
                    if (meshVNum[j] != 0)
                        pre[j].GetComponent<MeshRenderer>().materials[0].color = Color.black;
            }
            else
            {
                for (int j = 0; j < meshNum; j++)
                    if (meshVNum[j] != 0)
                        pre[j].GetComponent<MeshRenderer>().materials[0].color = new Color(1, 1, 1, 0);
            }
        }

        //Is Scalar
        if (menuItem.GetComponent<MenuItem>().isScalarOnChange)
        {
            if (menuItem.GetComponent<MenuItem>().isScalar)
                ScalarDisplay();
            else
                ScalarDestroy();
        }
        //Is Scalar Change(also means Is Scalar)
        if (menuItem.GetComponent<MenuItem>().isScalarChange)
            ScalarDisplay();

        //Is Vector
        if (menuItem.GetComponent<MenuItem>().isVectorOnChange)
        {
            if (menuItem.GetComponent<MenuItem>().isVector)
            {
                VectorCreate();
                VectorDisplay();
            }
            else
                VectorDestroy();
        }
        //Is Vector Change(also means Is Vector)
        if (menuItem.GetComponent<MenuItem>().isVectorChange)
            VectorDisplay();
        //Is Scale
        if (menuItem.GetComponent<MenuItem>().isScaleOnChange)
        {
            if (menuItem.GetComponent<MenuItem>().isScale)
                DeformationDisplay();
            else
                DeformationDestroy();
        }
        //Is Scale Change(also means Is Scale)
        if (menuItem.GetComponent<MenuItem>().isScaleChange)
            DeformationDisplay();

        //Is Time
        if (menuItem.GetComponent<MenuItem>().timeNow != timeNow)
        {
            if (menuItem.GetComponent<MenuItem>().isScalar)
                ScalarDisplay();
            if (menuItem.GetComponent<MenuItem>().isVector)
                VectorDisplay();
            if (menuItem.GetComponent<MenuItem>().isScale)
                DeformationDisplay();
            if (timeNow == 0)
            {
                Debug.Log(Time.time - ttt);////////////////////////////
                ttt = Time.time;

            }
        }

    }

    void ScalarDisplay()
    {
        panelColor.SetActive(true);
        long sizeChar = sizeof(char);
        long sizeF = sizeof(float);
        long sizeC = Marshal.SizeOf(typeof(Color));
        long jf;
        info sca;
        sca.name = new char[charLength];
        scalarNow = menuItem.GetComponent<MenuItem>().scalarIndex;
        timeNow = menuItem.GetComponent<MenuItem>().timeNow;
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[timeNow]))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                //标量绘制
                jf = sizeMesh[areaNow] + sizeScalar[areaNow] * scalarNow;
                accessor.ReadArray<char>(jf, sca.name, 0, charLength);
                jf += sizeChar * charLength;
                accessor.Read<float>(jf, out sca.max);
                jf += sizeF;
                accessor.Read<float>(jf, out sca.min);
                jf += sizeF;
                tMax.text = "Max:" + sca.max;
                tMin.text = "Min:" + sca.min;
                for (int i = 0; i < meshNum; i++)
                {
                    if (meshVNum[i] != 0)
                    {
                        pre[i].GetComponent<MeshRenderer>().materials[1].SetFloat("_AlphaScale", 1);
                        accessor.ReadArray<Color>(jf, colors[i], 0, meshVNum[i]);
                        jf += sizeC * meshVNum[i];
                        Mesh mesh = pre[i].GetComponent<MeshFilter>().mesh;
                        mesh.colors = colors[i];
                    }
                }
            }
        }
    }
    void ScalarDestroy()
    {
        panelColor.SetActive(false);
        for (int i = 0; i < meshNum; i++)
        {
            if (meshVNum[i] != 0)
                pre[i].GetComponent<MeshRenderer>().materials[1].SetFloat("_AlphaScale", 0);
        }
    }

    void VectorCreate()
    {
        panelArraw.SetActive(true);
        //矢量创建
        quats = new Quaternion[meshNum][];
        values = new float[meshNum][];
        preArraw = new GameObject[meshNum][];
        for (int i = 0; i < meshNum; i++)
        {
            if (meshVNum[i] != 0)
            {
                preArraw[i] = new GameObject[meshVNum[i]];
                quats[i] = new Quaternion[meshVNum[i]];
                values[i] = new float[meshVNum[i]];
                Vector3 v = objects.GetComponent<Transform>().position;
                Quaternion q = objects.GetComponent<Transform>().rotation;
                for (int j = 0; j < meshVNum[i]; ++j)
                {
                    preArraw[i][j] = (GameObject)Instantiate(arraw,v,q);
                    preArraw[i][j].transform.parent = GetComponent<Transform>();
                    preArraw[i][j].GetComponent<Transform>().localPosition = vertices[i][j];
                }
            }
        }
    }
    void VectorDisplay()
    {
        long sizeChar = sizeof(char);
        long sizeF = sizeof(float);
        long sizeQ = Marshal.SizeOf(typeof(Quaternion));
        long jf;
        info vec;
        vec.name = new char[charLength];
        vectorNow = menuItem.GetComponent<MenuItem>().vectorIndex;
        timeNow = menuItem.GetComponent<MenuItem>().timeNow;
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[timeNow]))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                //矢量绘制
                jf = sizeMesh[areaNow] + sizeScalar[areaNow] * scalarNum + sizeVector[areaNow] * vectorNow;
                accessor.ReadArray<char>(jf, vec.name, 0, charLength);
                jf += sizeChar * charLength;
                accessor.Read<float>(jf, out vec.max);
                jf += sizeF;
                accessor.Read<float>(jf, out vec.min);
                jf += sizeF;
                tMax1.text = "Max:" + vec.max;
                tMin1.text = "Min:" + vec.min;
                for (int i = 0; i < meshNum; i++)
                {
                    if (meshVNum[i] != 0)
                    {
                        accessor.ReadArray<Quaternion>(jf, quats[i], 0, meshVNum[i]);
                        jf += sizeQ * meshVNum[i];
                        accessor.ReadArray<float>(jf, values[i], 0, meshVNum[i]);
                        jf += sizeF * meshVNum[i];
                        Quaternion q = objects.GetComponent<Transform>().rotation;
                        for (int j = 0; j < meshVNum[i]; ++j)
                        {
                            preArraw[i][j].GetComponent<Transform>().rotation = q * quats[i][j];
                            preArraw[i][j].GetComponent<Transform>().localScale = new Vector3(1, 1, 1) * 0.1f * values[i][j];
                        }
                    }
                }
            }
        }
    }
    void VectorDestroy()
    {
        panelArraw.SetActive(false);
        for (int i = 0; i < meshNum; i++)
        {
            if (meshVNum[i] != 0)
            {

                for (int j = 0; j < meshVNum[i]; ++j)
                {
                    Destroy(preArraw[i][j]);
                }
            }
        }
    }

    void DeformationDisplay()
    {
        //变形绘制
        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long jf;
        timeNow = menuItem.GetComponent<MenuItem>().timeNow;
        float[] scales = new float[] { 0.5f,1,2,5};
        float scale = scales[menuItem.GetComponent<MenuItem>().scaleIndex];
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(files[timeNow]))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                jf = sizeMesh[areaNow] + sizeScalar[areaNow] * scalarNum + sizeVector[areaNow] * vectorNum;
                for (int j = 0; j < meshNum; j++)
                {
                    if (meshVNum[j] != 0)
                    {
                        Avertices[j] = new Vector3[meshVNum[j]];
                        accessor.ReadArray<Vector3>(jf, Avertices[j], 0, meshVNum[j]);
                        for (int k = 0; k < meshVNum[j]; k++)
                        {
                            Avertices[j][k] *= scale;
                            Avertices[j][k] += vertices[j][k];
                        }
                        Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                        mesh.vertices = Avertices[j];
                        mesh.RecalculateNormals();
                    }
                    jf += sizeV * meshVNum[j];
                }
            }
        }
    }
    void DeformationDestroy()
    {
        for (int j = 0; j < meshNum; j++)
        {
            if (meshVNum[j] != 0)
            {
                Mesh mesh = pre[j].GetComponent<MeshFilter>().mesh;
                mesh.vertices = vertices[j];
                mesh.RecalculateNormals();
            }
        }
    }


struct info
{
//长度暂定为30(charLength)
public char[] name;
public float max;
public float min;
}

}
