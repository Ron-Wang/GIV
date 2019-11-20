using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using GK;
using SFB;
using System.Threading.Tasks;

public class CreateFem : MonoBehaviour {
    public string[] inputFiles;
    public string outputFile;
    private string[] outputFiles;
    int timeNum;
    int domainNum;
    int inputScalarNum;
    int outputScalarNum;
    int charLength = 30;
    Quaternion q = Quaternion.identity;
    Vector3 v = new Vector3(0, 0, 1);
    string[] scaName = null,vecName = null;
    private List<int> scalarsList = new List<int>(), vectorsList = new List<int>();
    int[] scaNum = null,vecNum = null;
    private GameObject panelVar;//变量选择界面
    private Button ButtonOKSca;//变量选择界面的“确定”按钮
    private GameObject TextSca, TextVec, TextSec;
    private GameObject[] togglesSca, togglesVec;//变量选择界面的标量复选框、矢量复选框
    float Max = 0, Min = 0;
    float xMax = 0, xMin = 0,
        yMax = 0, yMin = 0,
        zMax = 0, zMin = 0;
    private GameObject dSection = null;
    private GameObject[] inpSection = null;
    private GameObject panelGiv;//可视化界面
    private GameObject newObject;
    private int numS = 0;
    int position = 0;
    private int DefIndex = 0;

    //只读取.vtk文件
    void Start () {
        panelVar = GameObject.Find("Canvas/PanelVar");
        panelGiv = GameObject.Find("Canvas/PanelGiv");
        panelVar.SetActive(false);
        panelGiv.SetActive(false);
        timeNum = inputFiles.Length;
        outputFiles = new string[timeNum];
        CreateGivWithoutDomains();
    }

    private void Update()
    {
        /*
        if (dSection != null)
        {
            if (numS != dSection.transform.GetComponent<Dropdown>().value)
            {
                for (int i = 0; i < numS; ++i)
                {
                    Destroy(inpSection[i]);
                }
                if (numS > 3)
                    position -= 2;
                else if (numS > 0)
                    position -= 1;
                numS = dSection.transform.GetComponent<Dropdown>().value;
                if (numS > 0)
                {
                    inpSection = new GameObject[numS];
                    for (int i = 0; i < numS; ++i)
                    {
                        if (i % 3 == 0)
                            ++position;
                        inpSection[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/SectionInp"));
                        inpSection[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
                        inpSection[i].transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
                        inpSection[i].transform.localPosition = new Vector3(-500 + 400 * (i % 3), 350 - 40 * position, 0);
                    }
                }
            }
        }
        */
    }

    public void CreateGivWithoutDomains()
    {
        List<string> listVtk;
        listVtk = LoadTextFile(inputFiles[0]);
        int line = 0;
        ArrayList scaArray = new ArrayList(),
            vecArray = new ArrayList();
        for (; line < listVtk.Count; ++line)
        {
            //寻找变量名
            if (Regex.IsMatch(listVtk[line], @"NodalFields/*"))
            {
                if (Regex.IsMatch(listVtk[line], @"SCALARS*"))//标量
                    scaArray.Add(new Regex(@"NodalFields/(\S+) ").Match(listVtk[line]).Groups[1].Value);
                else if (Regex.IsMatch(listVtk[line], @"VECTORS*"))//矢量
                    vecArray.Add(new Regex(@"NodalFields/(\S+) ").Match(listVtk[line]).Groups[1].Value);
                else if (int.Parse(listVtk[line].Split(' ')[1]) == 1)
                    scaArray.Add(new Regex(@"NodalFields/(\S+) ").Match(listVtk[line]).Groups[1].Value);
                else if (int.Parse(listVtk[line].Split(' ')[1]) == 3)
                    vecArray.Add(new Regex(@"NodalFields/(\S+) ").Match(listVtk[line]).Groups[1].Value);
            }
        }
        scaName = (string[])scaArray.ToArray(typeof(string));
        vecName = (string[])vecArray.ToArray(typeof(string));

        panelVar.SetActive(true);
        ButtonOKSca = (Button)GameObject.Find("Canvas/PanelVar/ButtonOK").GetComponent<Button>();
        TextSca = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Text"));
        TextSca.GetComponent<Text>().text = "  Scalars:";
        TextSca.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        TextSca.transform.localPosition = new Vector3(-500, 350, 0);
        TextSca.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale*2;
        togglesSca = new GameObject[scaName.Length];
        for (int i = 0; i < scaName.Length; ++i)
        {
            if (i % 4 == 0)
                ++position;
            togglesSca[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Toggle"));
            togglesSca[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
            togglesSca[i].transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale*2;
            togglesSca[i].transform.localPosition = new Vector3(-500 + 300 * (i % 4), 350 - 40 * position, 0);
            togglesSca[i].GetComponentInChildren<Text>().text = scaName[i];
        }
        ++position;
        TextVec = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Text"));
        TextVec.GetComponent<Text>().text = "  Vectors:";
        TextVec.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        TextVec.transform.localPosition = new Vector3(-500, 350 - 40 * position, 0);
        TextVec.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale*2;
        togglesVec = new GameObject[vecName.Length];
        for (int i = 0; i < vecName.Length; ++i)
        {
            if (i % 4 == 0)
                ++position;
            togglesVec[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Toggle"));
            togglesVec[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
            togglesVec[i].transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
            togglesVec[i].transform.localPosition = new Vector3(-500 + 300 * (i % 4), 350 - 40 * position, 0);
            togglesVec[i].GetComponentInChildren<Text>().text = vecName[i];
        }
        ++position;
        TextSec = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Text"));
        TextSec.GetComponent<Text>().text = "  Sections:";
        TextSec.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        TextSec.transform.localPosition = new Vector3(-500, 350 - 40 * position, 0);
        TextSec.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;

        ++position;
        dSection = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/DropdownNum"));
        dSection.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        dSection.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
        dSection.transform.localPosition = new Vector3(-500, 350 - 40 * position, 0);

        ButtonOKSca.onClick.AddListener(onClickOKSca);
    }



    //读取文本文件
    public List<string> LoadTextFile(string filePath)
    {
        List<string> listText = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    listText.Add(line);
                }
            }
        }
        catch (Exception e)
        {
            return null;
        }
        return listText;
    }

    //变量信息结构体（名称、最大值、最小值）
    struct info
    {
        //长度暂定为20(charLength)
        public char[] name;
        public float max;
        public float min;
    }

    //四色插值
    public Color FourColor(float value)
    {
        Color c;
        if (value < 1 / 3f)
            c = Color.Lerp(Color.blue, Color.green, 3 * value);
        else if (value < 2 / 3f)
            c = Color.Lerp(Color.green, Color.yellow, 3 * (value - 1 / 3f));
        else
            c = Color.Lerp(Color.yellow, Color.red, 3 * (value - 2 / 3f));
        return c;
    }

    //根据路径创建特定大小的文件
    public static void CreateFileWithSize(string fileName, long size)
    {
        string dir = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Create);
            fs.SetLength(size);
        }
        catch
        {
            if (fs != null)
            {
                fs.Close();
                File.Delete(fileName);
            }
            throw;
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }
    }

    void onClickOKSca()
    {
        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeC = Marshal.SizeOf(typeof(Color));
        long sizeQ = Marshal.SizeOf(typeof(Quaternion));
        long sizeF = sizeof(float);
        long sizeChar = sizeof(char);
        int areaNum = 1;
        areaNum += numS;
        string[] area = new string[areaNum];
        long sumSize = sizeB * 2 + sizeI * 4 + sizeChar * charLength * areaNum;
        area[0] = "Whole";

        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];

        //获取用户选择的变量
        for (int i = 0; i < scaName.Length; ++i)
        {
            if (togglesSca[i].GetComponent<Toggle>().isOn)
                scalarsList.Add(i);
        }
        scaNum = scalarsList.ToArray();
        for (int i = 0; i < vecName.Length; ++i)
        {
            if (togglesVec[i].GetComponent<Toggle>().isOn)
                vectorsList.Add(i);
        }
        vecNum = vectorsList.ToArray();
        panelVar.SetActive(false);

        //范围、节点Index、三角形Index
        List<string> listVtk;
        int verNum = 0, triNum = 0, cellNum = 0;
        float[] points = null;
        Vector3[] vertices = null;
        int[] triangles = null;
        float[][] scalars = null,
            vectors = null;
        float[] scalarsMax = null,
            scalarsMin = null,
            vectorsMax = null,
            vectorsMin = null;
        Vector3[] verAdd = null;

        bool changeMesh = false;
        int meshNum = 1;
        Vector3 maxVertice, minVertice;
        info[] scaInfo;
        Color[][] scaColor;
        info[] vecInfo;
        Quaternion[][] vecQua;
        float[][] vecValue;
        bool isVerAdd = false;

        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        scalars = new float[scalarsNum][];
        scalarsMax = new float[scalarsNum];
        scalarsMin = new float[scalarsNum];
        vectors = new float[vectorsNum][];
        vectorsMax = new float[vectorsNum];
        vectorsMin = new float[vectorsNum];

        int line = 0;
        listVtk = LoadTextFile(inputFiles[0]);
        for (; line < listVtk.Count; ++line)
        {
            //边界最大最小值
            //if (Regex.IsMatch(listVtk[line], @"^avtOriginalBounds"))
            if(listVtk[line].Contains("avtOriginalBounds "))
            {
                string[] lineToArray = listVtk[line + 1].Split(' ');
                xMin = float.Parse(lineToArray[0]);
                xMax = float.Parse(lineToArray[1]);
                yMin = float.Parse(lineToArray[2]);
                yMax = float.Parse(lineToArray[3]);
                zMin = float.Parse(lineToArray[4]);
                zMax = float.Parse(lineToArray[5]);
            }
            //节点
            //if (Regex.IsMatch(listVtk[line], @"^POINTS"))
            if (listVtk[line].Contains("POINTS "))
            {
                string[] lineToArray = listVtk[line].Split(' ');
                verNum = int.Parse(lineToArray[1]);
                ++line;
                points = new float[3 * verNum];
                int pointIndex = 0;
                for (; line < listVtk.Count; ++line)
                {
                    lineToArray = listVtk[line].Split(' ');
                    for (int i = 0; i < lineToArray.Length - 1; ++i)
                    {
                        points[pointIndex] = float.Parse(lineToArray[i]);
                        ++pointIndex;
                    }
                    if (pointIndex == 3 * verNum)
                        break;
                }
            }
            //单元
            //if (Regex.IsMatch(listVtk[line], @"^CELLS"))
            if (listVtk[line].Contains("CELLS "))
            {
                string[] lineToArray = listVtk[line].Split(' ');
                cellNum = int.Parse(lineToArray[1]);
                ++line;
                if (int.Parse(listVtk[line].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                {
                    triNum = 4 * cellNum;
                    triangles = new int[3 * triNum];
                    int triIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        triangles[triIndex] = int.Parse(lineToArray[1]);
                        triangles[triIndex + 1] = int.Parse(lineToArray[2]);
                        triangles[triIndex + 2] = int.Parse(lineToArray[3]);
                        triangles[triIndex + 3] = int.Parse(lineToArray[1]);
                        triangles[triIndex + 4] = int.Parse(lineToArray[3]);
                        triangles[triIndex + 5] = int.Parse(lineToArray[4]);
                        triangles[triIndex + 6] = int.Parse(lineToArray[1]);
                        triangles[triIndex + 7] = int.Parse(lineToArray[4]);
                        triangles[triIndex + 8] = int.Parse(lineToArray[2]);
                        triangles[triIndex + 9] = int.Parse(lineToArray[2]);
                        triangles[triIndex + 10] = int.Parse(lineToArray[4]);
                        triangles[triIndex + 11] = int.Parse(lineToArray[3]);
                        triIndex += 12;
                        if (triIndex == 3 * triNum)
                            break;
                    }
                }
            }
        }
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        vertices = new Vector3[verNum];
        scaInfo = new info[scalarsNum];
        scaColor = new Color[scalarsNum][];
        vecInfo = new info[vectorsNum];
        vecQua = new Quaternion[vectorsNum][];
        vecValue = new float[vectorsNum][];
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        //节点归一化
        float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;

        Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
        //for (int verIndex = 0; verIndex < verNum; ++verIndex)
        {
            vertices[verIndex] = new Vector3(10 * (points[3 * verIndex] - xx) / (Max - Min),
                    -10 * (points[3 * verIndex + 1] - yy) / (Max - Min),
                    10 * (points[3 * verIndex + 2] - zz) / (Max - Min));
        });
        sumSize += sizeI * 2 + sizeV * 2 + sizeV * verNum + sizeI * triNum * 3;
        outputFiles[0] = outputFile.Replace(".giv", "") + "0.giv";//输出文件路径

        Section[] section = new Section[numS];
        string[] Option = new string[] { "X = ", "Y = ", "Z = " };
        for (int i = 0; i < numS; ++i)
        {
            int sectionOption = inpSection[i].GetComponentInChildren<Dropdown>().value;
            float sectionValue = float.Parse(inpSection[i].GetComponentInChildren<InputField>().text);
            switch (sectionOption)
            {
                case 0:
                    section[i] = new Section(points, triangles, sectionOption, xMin + (xMax - xMin) * sectionValue, maxVertice, minVertice);
                    break;
                case 1:
                    section[i] = new Section(points, triangles, sectionOption, yMin + (yMax - yMin) * sectionValue, maxVertice, minVertice);
                    break;
                case 2:
                    section[i] = new Section(points, triangles, sectionOption, zMin + (zMax - zMin) * sectionValue, maxVertice, minVertice);
                    break;
            }
            if (section[i].nodeNum > 0)
                sumSize += sizeI * 2 + sizeV * 2 + sizeV * section[i].nodeNum + sizeI * section[i].triangles.Length;
            else
                sumSize += sizeI * 2 + sizeV * 2;
            area[1 + i] = Option[sectionOption] + sectionValue;

        }

        for (int time = 0; time < timeNum; ++time)
        {
            listVtk = LoadTextFile(inputFiles[time]);
            line = 0;
            int scaIndex = 0, vecIndex = 0;
            for (; line < listVtk.Count; ++line)
            {
                //标量
                //if (scaIndex < scalarsNum && Regex.IsMatch(listVtk[line], scaName[scaNum[scaIndex]] + " "))
                if (scaIndex < scalarsNum && listVtk[line].Contains(scaName[scaNum[scaIndex]] + " "))
                {
                    ++line;
                    if (listVtk[line].Split(' ')[1] == "default")
                        ++line;
                    scalars[scaIndex] = new float[verNum];
                    int scalarIndex = 0;
                    scalarsMax[scaIndex] = float.MinValue;
                    scalarsMin[scaIndex] = float.MaxValue;

                    for (; line < listVtk.Count; ++line)
                    {
                        string[] lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            scalars[scaIndex][scalarIndex] = float.Parse(lineToArray[i]);
                            scalarsMax[scaIndex] = Mathf.Max(scalarsMax[scaIndex], scalars[scaIndex][scalarIndex]);
                            scalarsMin[scaIndex] = Mathf.Min(scalarsMin[scaIndex], scalars[scaIndex][scalarIndex]);
                            ++scalarIndex;
                        }
                        if (scalarIndex == verNum)
                            break;
                    }
                    ++scaIndex;
                }
                //矢量
                //else if (vecIndex < vectorsNum && Regex.IsMatch(listVtk[line], vecName[vecNum[vecIndex]] + " "))
                else if (vecIndex < vectorsNum && listVtk[line].Contains(vecName[vecNum[vecIndex]] + " "))
                {
                    ++line;
                    if (listVtk[line].Split(' ')[1] == "default")
                        ++line;
                    vectors[vecIndex] = new float[3 * verNum];
                    int vectorIndex = 0;
                    vectorsMax[vecIndex] = float.MinValue;
                    vectorsMin[vecIndex] = float.MaxValue;
                    for (; line < listVtk.Count; ++line)
                    {
                        string[] lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            vectors[vecIndex][vectorIndex] = float.Parse(lineToArray[i]);
                            if (vectorIndex % 3 == 2)
                            if (vectorIndex == 2)
                            {
                                vectorsMax[vecIndex] = Mathf.Sqrt(vectors[vecIndex][vectorIndex] * vectors[vecIndex][vectorIndex]
                                    + vectors[vecIndex][vectorIndex - 1] * vectors[vecIndex][vectorIndex - 1]
                                    + vectors[vecIndex][vectorIndex - 2] * vectors[vecIndex][vectorIndex - 2]);
                                vectorsMin[vecIndex] = vectorsMax[vecIndex];
                            }
                            else if (vectorIndex % 3 == 2)
                            {
                                vectorsMax[vecIndex] = Mathf.Max(vectorsMax[vecIndex],
                                    Mathf.Sqrt(vectors[vecIndex][vectorIndex] * vectors[vecIndex][vectorIndex]
                                    + vectors[vecIndex][vectorIndex - 1] * vectors[vecIndex][vectorIndex - 1]
                                    + vectors[vecIndex][vectorIndex - 2] * vectors[vecIndex][vectorIndex - 2]));
                                vectorsMin[vecIndex] = Mathf.Min(vectorsMin[vecIndex],
                                    Mathf.Sqrt(vectors[vecIndex][vectorIndex] * vectors[vecIndex][vectorIndex]
                                    + vectors[vecIndex][vectorIndex - 1] * vectors[vecIndex][vectorIndex - 1]
                                    + vectors[vecIndex][vectorIndex - 2] * vectors[vecIndex][vectorIndex - 2]));
                            }
                            ++vectorIndex;
                        }
                        if (vectorIndex == 3 * verNum)
                            break;
                    }
                    ++vecIndex;
                }
            }
            //标量信息结构体初始化、颜色归一化
            for (int scaIndex1 = 0; scaIndex1 < scalarsNum; ++scaIndex1)
            {
                scaInfo[scaIndex1].max = scalarsMax[scaIndex1];
                scaInfo[scaIndex1].min = scalarsMin[scaIndex1];
                scaInfo[scaIndex1].name = new char[charLength];
                for (int j = 0; j < charLength; j++)
                {
                    if (j < scaName[scaNum[scaIndex1]].Length)
                        scaInfo[scaIndex1].name[j] = scaName[scaNum[scaIndex1]][j];
                    else
                        scaInfo[scaIndex1].name[j] = ' ';
                }
                scaColor[scaIndex1] = new Color[verNum];
                if (scalarsMax[scaIndex1] == scalarsMin[scaIndex1])
                {
                    Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                    //for (int verIndex = 0; verIndex < verNum; ++verIndex)
                    {
                        scaColor[scaIndex1][verIndex] = Color.blue;
                    });
                }
                else
                {
                    Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                    //for (int verIndex = 0; verIndex < verNum; ++verIndex)
                    {
                        scaColor[scaIndex1][verIndex] = FourColor((scalars[scaIndex1][verIndex] - scalarsMin[scaIndex1])
                            / (scalarsMax[scaIndex1] - scalarsMin[scaIndex1]));
                    });
                }
                if (time == 0)
                {
                    sumSize += sizeChar * charLength + sizeF * 2 + sizeC * verNum;
                    for (int ii = 0; ii < numS; ++ii)
                        sumSize += sizeChar * charLength + sizeF * 2 + sizeC * section[ii].nodeNum;
                }
            }
            //矢量信息结构体初始化、方向大小归一化
            for (int vecIndex1 = 0; vecIndex1 < vectorsNum; ++vecIndex1)
            {
                vecInfo[vecIndex1].max = vectorsMax[vecIndex1];
                vecInfo[vecIndex1].min = vectorsMin[vecIndex1];
                vecInfo[vecIndex1].name = new char[charLength];
                for (int j = 0; j < charLength; j++)
                {
                    if (j < vecName[vecNum[vecIndex1]].Length)
                        vecInfo[vecIndex1].name[j] = vecName[vecNum[vecIndex1]][j];
                    else
                        vecInfo[vecIndex1].name[j] = ' ';
                }
                vecQua[vecIndex1] = new Quaternion[verNum];
                vecValue[vecIndex1] = new float[verNum];

                if (vectorsMax[vecIndex1] == vectorsMin[vecIndex1])
                {
                    Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                    //for (int verIndex = 0; verIndex < verNum; ++verIndex)
                    {
                        q.SetFromToRotation(v, new Vector3(vectors[vecIndex1][3 * verIndex],
                            vectors[vecIndex1][3 * verIndex + 1], vectors[vecIndex1][3 * verIndex + 2]));
                        vecQua[vecIndex1][verIndex] = q;
                        vecValue[vecIndex1][verIndex] = 0.1f;
                    });
                }
                else
                {
                    Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                    //for (int verIndex = 0; verIndex < verNum; ++verIndex)
                    {
                        q.SetFromToRotation(v, new Vector3(vectors[vecIndex1][3 * verIndex],
                            vectors[vecIndex1][3 * verIndex + 1], vectors[vecIndex1][3 * verIndex + 2]));
                        vecQua[vecIndex1][verIndex] = q;
                        vecValue[vecIndex1][verIndex] =
                            (Mathf.Sqrt(vectors[vecIndex1][3 * verIndex] * vectors[vecIndex1][3 * verIndex]
                                    + vectors[vecIndex1][3 * verIndex + 1] * vectors[vecIndex1][3 * verIndex + 1]
                                    + vectors[vecIndex1][3 * verIndex + 2] * vectors[vecIndex1][3 * verIndex + 2])
                            - vectorsMin[vecIndex1]) / (vectorsMax[vecIndex1] - vectorsMin[vecIndex1]);
                    });
                }
                //附加位移
                if (vecName[vecNum[vecIndex1]] == "Displacement")
                {
                    isVerAdd = true;
                    DefIndex = vecIndex1;
                    verAdd = new Vector3[verNum];
                    Parallel.For(0, verNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                    //for (int verIndex = 0; verIndex < verNum; ++verIndex)
                    {
                        verAdd[verIndex] = new Vector3(1000 * vectors[vecIndex1][3 * verIndex] / (Max - Min),
                            1000 * vectors[vecIndex1][3 * verIndex + 1] / (Max - Min),
                            1000 * vectors[vecIndex1][3 * verIndex + 2] / (Max - Min));
                    });
                    if (time == 0)
                    {
                        sumSize += sizeV * verNum;
                        for (int ii = 0; ii < numS; ++ii)
                            sumSize += sizeV * section[ii].nodeNum;
                    }
                }
                if (time == 0)
                {
                    sumSize += sizeChar * charLength + sizeF * 2 + (sizeQ + sizeF) * verNum;
                    for (int ii = 0; ii < numS; ++ii)
                        sumSize += sizeChar * charLength + sizeF * 2 + (sizeQ + sizeF) * section[ii].nodeNum;
                }
            }


            outputFiles[time] = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles[time], sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[time]))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref meshNum);
                    jf += sizeI;
                    accessor.Write<int>(jf, ref scalarsNum);
                    jf += sizeI;
                    accessor.Write<int>(jf, ref vectorsNum);
                    jf += sizeI;
                    accessor.Write<bool>(jf, ref isVerAdd);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref areaNum);
                    jf += sizeI;
                    for (int areaIndex = 0; areaIndex < areaNum; ++areaIndex)
                    {
                        char[] areaName = new char[charLength];
                        for (int j = 0; j < charLength; j++)
                        {
                            if (j < area[areaIndex].Length)
                            {
                                areaName[j] = area[areaIndex][j];
                            }
                            else
                                areaName[j] = ' ';
                        }
                        accessor.WriteArray<char>(jf, areaName, 0, charLength);
                        jf += sizeChar * charLength;
                    }
                    //Whole 
                    accessor.Write<Vector3>(jf, ref maxVertice);
                    jf += sizeV;
                    accessor.Write<Vector3>(jf, ref minVertice);
                    jf += sizeV;
                    accessor.Write<int>(jf, ref verNum);
                    jf += sizeI;
                    int triVerNum = 3 * triNum;
                    accessor.Write<int>(jf, ref triVerNum);
                    jf += sizeI;
                    accessor.WriteArray<Vector3>(jf, vertices, 0, verNum);
                    jf += sizeV * verNum;
                    accessor.WriteArray<int>(jf, triangles, 0, triVerNum);
                    jf += sizeI * triVerNum;
                    for (int scaIndex1 = 0; scaIndex1 < scalarsNum; ++scaIndex1)
                    {
                        accessor.WriteArray<char>(jf, scaInfo[scaIndex1].name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo[scaIndex1].max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo[scaIndex1].min);
                        jf += sizeF;
                        accessor.WriteArray<Color>(jf, scaColor[scaIndex1], 0, verNum);
                        jf += sizeC * verNum;
                    }
                    for (int vecIndex1 = 0; vecIndex1 < vectorsNum; ++vecIndex1)
                    {
                        accessor.WriteArray<char>(jf, vecInfo[vecIndex1].name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo[vecIndex1].max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo[vecIndex1].min);
                        jf += sizeF;
                        accessor.WriteArray<Quaternion>(jf, vecQua[vecIndex1], 0, verNum);
                        jf += sizeQ * verNum;
                        accessor.WriteArray<float>(jf, vecValue[vecIndex1], 0, verNum);
                        jf += sizeF * verNum;
                    }
                    if (isVerAdd)
                    {
                        accessor.WriteArray<Vector3>(jf, verAdd, 0, verNum);
                        jf += sizeV * verNum;
                    }
                    for (int areaIndex = 0; areaIndex < numS; ++areaIndex)
                    {
                        //Section
                        accessor.Write<Vector3>(jf, ref maxVertice);
                        jf += sizeV;
                        accessor.Write<Vector3>(jf, ref minVertice);
                        jf += sizeV;
                        int SectionVerNum = section[areaIndex].nodeNum;
                        accessor.Write<int>(jf, ref SectionVerNum);
                        jf += sizeI;
                        int SectionTriVerNum = section[areaIndex].triangles.Length;
                        accessor.Write<int>(jf, ref SectionTriVerNum);
                        jf += sizeI;
                        accessor.WriteArray<Vector3>(jf, section[areaIndex].vertices, 0, SectionVerNum);
                        jf += sizeV * SectionVerNum;
                        accessor.WriteArray<int>(jf, section[areaIndex].triangles, 0, SectionTriVerNum);
                        jf += sizeI * SectionTriVerNum;
                        for (int scaIndex1 = 0; scaIndex1 < scalarsNum; ++scaIndex1)
                        {
                            Color[] SectionSca = null;
                            float SectionSMax = 0, SectionSMin = 0;
                            section[areaIndex].Scalars(scalars[scaIndex1], ref SectionSca, ref SectionSMax, ref SectionSMin);
                            accessor.WriteArray<char>(jf, scaInfo[scaIndex1].name, 0, charLength);
                            jf += sizeChar * charLength;
                            accessor.Write<float>(jf, ref SectionSMax);
                            jf += sizeF;
                            accessor.Write<float>(jf, ref SectionSMin);
                            jf += sizeF;
                            accessor.WriteArray<Color>(jf, SectionSca, 0, SectionVerNum);
                            jf += sizeC * SectionVerNum;
                        }
                        for (int vecIndex1 = 0; vecIndex1 < vectorsNum; ++vecIndex1)
                        {
                            Quaternion[] SectionQ = null;
                            float[] SectionValue = null;
                            float SectionVMax = 0, SectionVMin = 0;
                            section[areaIndex].VectorsQ(vectors[vecIndex1],ref SectionQ,ref SectionValue,ref SectionVMax,ref SectionVMin);
                            accessor.WriteArray<char>(jf, vecInfo[vecIndex1].name, 0, charLength);
                            jf += sizeChar * charLength;
                            accessor.Write<float>(jf, ref SectionVMax);
                            jf += sizeF;
                            accessor.Write<float>(jf, ref SectionVMin);
                            jf += sizeF;
                            accessor.WriteArray<Quaternion>(jf, SectionQ, 0, SectionVerNum);
                            jf += sizeQ * SectionVerNum;
                            accessor.WriteArray<float>(jf, SectionValue, 0, SectionVerNum);
                            jf += sizeF * SectionVerNum;
                        }
                        if (isVerAdd)
                        {
                            Vector3[] SectionDef = null;
                            section[areaIndex].Deformation(vectors[DefIndex],ref SectionDef);
                            accessor.WriteArray<Vector3>(jf, SectionDef, 0, SectionVerNum);
                            jf += sizeV * SectionVerNum;
                        }
                    }
                }
            }
        }
        panelGiv.SetActive(true);//可视化界面显示
        newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/ReadGiv"));
        newObject.GetComponent<ReadGiv>().files = outputFiles;
        newObject.transform.parent = GetComponent<Transform>();
    }

}
