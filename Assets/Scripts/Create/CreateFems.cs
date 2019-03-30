using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class CreateFems : MonoBehaviour {
    public string[] inputFiles;
    private string[] inputFilePathes;
    private string inputFilePathDir;
    public string outputFile;
    private string[] outputFiles;
    int timeNum;
    int domainNum;
    int charLength = 30;
    Quaternion q = Quaternion.identity;
    Vector3 v = new Vector3(0, 0, 1);
    string[] scaName = null,vecName = null;
    private List<int> scalarsList = new List<int>(), vectorsList = new List<int>();
    int[] scaNum = null,vecNum = null;
    private GameObject panelVar;//变量选择界面
    private Button ButtonOKSca;//变量选择界面的“确定”按钮
    private GameObject[] togglesSca, togglesVec;//变量选择界面的标量复选框、矢量复选框
    private GameObject panelGiv;//可视化界面
    private GameObject newObject;

    //只读取.vtk文件
    void Start () {
        panelVar = GameObject.Find("Canvas/PanelVar");
        panelGiv = GameObject.Find("Canvas/PanelGiv");
        panelVar.SetActive(false);
        panelGiv.SetActive(false);

        domainNum = inputFiles.Length;//分区数
		inputFilePathDir = Path.GetDirectoryName(Path.GetDirectoryName(inputFiles[0]));
		inputFilePathes = Directory.GetDirectories(inputFilePathDir);
		timeNum = inputFilePathes.Length;
        outputFiles = new string[timeNum];
        if (timeNum == 0)
        {

        }
        else
        {
            CreateGivWithDomains();
        }
    }
    public void CreateGivWithDomains()
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
        togglesSca = new GameObject[scaName.Length];
        for (int i = 0; i < scaName.Length; ++i)
        {
            togglesSca[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Toggle"));
            togglesSca[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), false);
            togglesSca[i].transform.position = new Vector3(100 * i + 300, 500, 0);
            togglesSca[i].GetComponentInChildren<Text>().text = scaName[i];
        }
        togglesVec = new GameObject[vecName.Length];
        for (int i = 0; i < vecName.Length; ++i)
        {
            togglesVec[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Toggle"));
            togglesVec[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), false);
            togglesVec[i].transform.position = new Vector3(100 * i + 100, 300, 0);
            togglesVec[i].GetComponentInChildren<Text>().text = vecName[i];
        }
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

    //将数组置为同一个数
    void SetArrayValue(float[] array, float value)
    {
        for (int i = 0; i < array.Length; ++i)
            array[i] = value;
    }

    void onClickOKSca()
    {
        //获取用户选择的变量
        for (int i = 0; i < scaName.Length; ++i)
        {
            if (togglesSca[i].GetComponent<Toggle>().isOn)
                scalarsList.Add(i);
        }
        scaNum = scalarsList.ToArray();
        for (int i = 0; i < vecName.Length; ++i)
        {
            if (togglesSca[i].GetComponent<Toggle>().isOn)
                vectorsList.Add(i);
        }
        vecNum = vectorsList.ToArray();
        panelVar.SetActive(false);

        //范围、节点Index、三角形Index
        List<string> listVtk;
        float xMax = 0, xMin = 0, yMax = 0, yMin = 0, zMax = 0, zMin = 0;
		int[] verDomainNum,triDomainNum;
		int[][] verDomainIndex,triDomain;
        float[][] points;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
		triDomain = new int[domainNum][];
        points = new float[domainNum][];

        Vector3[][] vertices = null;


        float[][] scalars = null,
            vectors = null;
        float scalarsMax = 0,
            scalarsMin = 0,
            vectorsMax = 0,
            vectorsMin = 0;
        Vector3[][] verAdd = null;

        bool changeMesh = false;
        int meshNum = 0;
        Vector3 maxVertice, minVertice;
        info scaInfo;
        Color[][] scaColor;
        info vecInfo;
        Quaternion[][] vecQua;
        float[][] vecValue;
        bool isVerAdd = false;

        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeC = Marshal.SizeOf(typeof(Color));
        long sizeQ = Marshal.SizeOf(typeof(Quaternion));
        long sizeF = sizeof(float);
        long sizeChar = sizeof(char);

        long sumSize = sizeB + sizeI * 3 + sizeV * 2;


        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;

        for (int domain = 0; domain < domainNum; ++domain)
        {
            listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            for (; line < listVtk.Count; ++line)
            {
                //x,y,z的范围
                if (Regex.IsMatch(listVtk[line], @"^avtOriginalBounds"))
                {

                    if (domain == 0)
                    {
                        string[] lineToArray = listVtk[line + 1].Split(' ');
                        xMin = float.Parse(lineToArray[0]);
                        xMax = float.Parse(lineToArray[1]);
                        yMin = float.Parse(lineToArray[2]);
                        yMax = float.Parse(lineToArray[3]);
                        zMin = float.Parse(lineToArray[4]);
                        zMax = float.Parse(lineToArray[5]);
                    }
                    else
                    {
                        string[] lineToArray = listVtk[line + 1].Split(' ');
                        xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                        xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                        yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                        yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                        zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                        zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    }
                }
                //1.读入节点
                int verNum = 0;
                if (Regex.IsMatch(listVtk[line], @"^POINTS"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line;
                    points[meshNum] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[meshNum][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                }
                //2.读入单元--转化为三角形--
                int triNum = 0;
                int[] triangles = null;
                if (Regex.IsMatch(listVtk[line], @"^CELLS"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
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

                //3.读入isExternal和ghostRank
                float[] isExternal = new float[verNum];
                SetArrayValue(isExternal, 1);
                float[] ghostRank = new float[verNum];
                SetArrayValue(ghostRank, -1);
                if (Regex.IsMatch(listVtk[line], @"^isExternal"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    ++line;
                    if (listVtk[line].Split(' ')[1] == "default")
                        ++line;
                    int isExternalIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                }
                if (Regex.IsMatch(listVtk[line], @"^ghostRank"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    ++line;
                    if (listVtk[line].Split(' ')[1] == "default")
                        ++line;
                    int ghostRankIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                }
                List<int> arrayTri = new List<int>();
                List<int> arrayVerIndex = new List<int>();
                int[] markVer = new int[verNum];
                int verNumNew = 0;
                //4.根据IsExternal和ghostRank挑选外部的节点和单元
                for (int triIndex = 0; triIndex < triNum; ++triIndex)
                {
                    if (isExternal[triangles[3 * triIndex]] == 1 && isExternal[triangles[3 * triIndex + 1]] == 1 && isExternal[triangles[3 * triIndex + 2]] == 1)
                    {
                        if (ghostRank[triangles[3 * triIndex]] < 0 || ghostRank[triangles[3 * triIndex + 1]] < 0 || ghostRank[triangles[3 * triIndex + 2]] < 0)
                        {
                            if (ghostRank[triangles[3 * triIndex]] < domain && ghostRank[triangles[3 * triIndex + 1]] < domain && ghostRank[triangles[3 * triIndex + 2]] < domain)
                            {
                                if (markVer[triangles[3 * triIndex]] == 0)
                                {
                                    arrayVerIndex.Add(triangles[3 * triIndex]);
                                    markVer[triangles[3 * triIndex]] = verNumNew;
                                    arrayTri.Add(verNumNew);
                                    ++verNumNew;
                                }
                                else
                                    arrayTri.Add(markVer[triangles[3 * triIndex]]);
                                if (markVer[triangles[3 * triIndex + 1]] == 0)
                                {
                                    arrayVerIndex.Add(triangles[3 * triIndex + 1]);
                                    markVer[triangles[3 * triIndex + 1]] = verNumNew;
                                    arrayTri.Add(verNumNew);
                                    ++verNumNew;
                                }
                                else
                                    arrayTri.Add(markVer[triangles[3 * triIndex + 1]]);
                                if (markVer[triangles[3 * triIndex + 2]] == 0)
                                {
                                    arrayVerIndex.Add(triangles[3 * triIndex + 2]);
                                    markVer[triangles[3 * triIndex + 2]] = verNumNew;
                                    arrayTri.Add(verNumNew);
                                    ++verNumNew;
                                }
                                else
                                    arrayTri.Add(markVer[triangles[3 * triIndex + 2]]);
                            }
                        }
                    }
                }
                verDomainNum[domain] = verNumNew;
                ///////////////////////////////
                if (verNumNew != 0)
                {
                    triDomain[domain] = arrayTri.ToArray();
                    triDomainNum[domain] = triDomain[domain].Length / 3;
                    verDomainIndex[domain] = arrayVerIndex.ToArray();
                    ++meshNum;
                }

            }
        }
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        vertices = new Vector3[domainNum][];
        //节点归一化
        for (int domain = 0; domain < domainNum; ++domain)
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xMin) / (xMax - xMin) - 5,
                        5 - 10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - yMin) / (yMax - yMin),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - zMin) / (zMax - zMin) - 5);
                }
            }
        }
        scaColor = new Color[meshNum][];
        vecQua = new Quaternion[meshNum][];
        vecValue = new float[meshNum][];

        //寻最值
        for (int time = 0; time < timeNum; ++time)
        {


            long jf = 0;
            CreateFileWithSize(outputFiles[time], sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[time]))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {

                }
            }



                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
            {
                scalars = new float[domainNum][];
                scaColor = new Color[domainNum][];
                float scaMax = 0, scaMin = 0;
                bool isMaxMin = true;
                for (int domain = 0; domain < domainNum; ++domain)
                {
                    if (verDomainNum[domain] != 0)
                    {
                        int verNum = 0;
                        listVtk = LoadTextFile(inputFiles[domain]);//获取文件夹文件名——————————————————————————————————————————————————————————————————————————————
                        int line = 0;
                        if (Regex.IsMatch(listVtk[line], @"^POINTS"))
                        {
                            string[] lineToArray = listVtk[line].Split(' ');
                            verNum = int.Parse(lineToArray[1]);
                        }
                        float[] value = new float[verNum];
                        int scalarIndex = 0;
                        for (; line < listVtk.Count; ++line)
                        {
                            if (Regex.IsMatch(listVtk[line], scaName[scaNum[scaIndex]] + " *"))
                            {
                                for (; line < listVtk.Count; ++line)
                                {
                                    string[] lineToArray = listVtk[line].Split(' ');
                                    for (int i = 0; i < lineToArray.Length - 1; ++i)
                                    {
                                        value[scalarIndex] = float.Parse(lineToArray[i]);
                                        ++scalarIndex;
                                    }
                                    if (scalarIndex == verNum)
                                        break;
                                }
                            }
                        }
                        scalars[domain] = new float[verDomainNum[domain]];
                        for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                        {
                            scalars[domain][verIndex] = value[verDomainIndex[domain][verIndex]];
                            if (isMaxMin)
                            {
                                scaMax = scalars[domain][verIndex];
                                scaMin = scalars[domain][verIndex];
                                isMaxMin = false;
                            }
                            else
                            {
                                scaMax = Mathf.Max(scalars[domain][verIndex],scaMax);
                                scaMin = Mathf.Min(scalars[domain][verIndex],scaMin);
                            }
                        }

                    }
                }

                for (int domain = 0; domain < domainNum; ++domain)
                {
                    if (verDomainNum[domain] != 0)
                    {
                        scaInfo.max = scaMax;
                        scaInfo.min = scaMin;
                        scaInfo.name = new char[charLength];
                        for (int j = 0; j < charLength; j++)
                        {
                            if (j < scaName[scaNum[scaIndex]].Length)
                                scaInfo.name[j] = scaName[scaNum[scaIndex]][j];
                            else
                                scaInfo.name[j] = ' ';
                        }
                        scaColor[domain] = new Color[verDomainNum[domain]];
                        if (scaMax == scaMin)
                        {
                            for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                            {
                                scaColor[domain][verIndex] = Color.blue;
                            }
                        }
                        else
                        {
                            for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                            {
                                scaColor[domain][verIndex] = FourColor((scalars[domain][verIndex] - scaMin) / (scaMax - scaMin));
                            }
                        }
                    }
                }
            }
            //矢量
            for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
            {
                vectors = new float[domainNum][];
                vecValue = new float[domainNum][];
                vecQua = new Quaternion[domainNum][];
                float vecMax = 0, vecMin = 0;
                bool isMaxMin = true;
                for (int domain = 0; domain < domainNum; ++domain)
                {
                    if (verDomainNum[domain] != 0)
                    {
                        int verNum = 0;
                        listVtk = LoadTextFile(inputFiles[domain]);//获取文件夹文件名——————————————————————————————————————————————————————————————————————————————
                        int line = 0;
                        if (Regex.IsMatch(listVtk[line], @"^POINTS"))
                        {
                            string[] lineToArray = listVtk[line].Split(' ');
                            verNum = int.Parse(lineToArray[1]);
                        }
                        float[] value = new float[3 * verNum];
                        int vectorIndex = 0;
                        for (; line < listVtk.Count; ++line)
                        {
                            if (Regex.IsMatch(listVtk[line], vecName[vecNum[vecIndex]] + " *"))
                            {
                                for (; line < listVtk.Count; ++line)
                                {
                                    string[] lineToArray = listVtk[line].Split(' ');
                                    for (int i = 0; i < lineToArray.Length - 1; ++i)
                                    {
                                        value[vectorIndex] = float.Parse(lineToArray[i]);
                                        ++vectorIndex;
                                    }
                                    if (vectorIndex == 3 * verNum)
                                        break;
                                }
                            }
                        }
                        vectors[domain] = new float[verDomainNum[domain]];
                        vecQua[domain] = new Quaternion[verDomainNum[domain]];
                        for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                        {
                            q.SetFromToRotation(v, new Vector3(value[3 * verDomainIndex[domain][verIndex]],
                            value[3 * verDomainIndex[domain][verIndex] + 1], value[3 * verDomainIndex[domain][verIndex] + 2]));
                            vecQua[domain][verIndex] = q;
                            vectors[domain][verIndex] = Mathf.Sqrt(value[3*verDomainIndex[domain][verIndex]]* value[3 * verDomainIndex[domain][verIndex]]
                                + value[3 * verDomainIndex[domain][verIndex] + 1] * value[3 * verDomainIndex[domain][verIndex] + 1]
                                + value[3 * verDomainIndex[domain][verIndex] + 2] * value[3 * verDomainIndex[domain][verIndex] + 2]
                                );
                            if (isMaxMin)
                            {
                                vecMax = vectors[domain][verIndex];
                                vecMin = vectors[domain][verIndex];
                                isMaxMin = false;
                            }
                            else
                            {
                                vecMax = Mathf.Max(vectors[domain][verIndex], vecMax);
                                vecMin = Mathf.Min(vectors[domain][verIndex], vecMin);
                            }
                        }
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                        {
                            isVerAdd = true;
                            verAdd = new Vector3[domainNum][];
                            verAdd[domain] = new Vector3[verDomainNum[domain]];
                            for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                            {
                                verAdd[domain][verIndex] = new Vector3((value[3 * verDomainIndex[domain][verIndex]] - xMin) / (xMax - xMin),
                                    (value[3 * verDomainIndex[domain][verIndex] + 1] - yMin) / (yMax - yMin),
                                    (value[3 * verDomainIndex[domain][verIndex] + 2] - zMin) / (zMax - zMin));
                            }
                            sumSize += sizeV * verNum;
                        }
                    }
                }
                for (int domain = 0; domain < domainNum; ++domain)
                {
                    if (verDomainNum[domain] != 0)
                    {
                        vecInfo.max = vecMax;
                        vecInfo.min = vecMin;
                        vecInfo.name = new char[charLength];
                        for (int j = 0; j < charLength; j++)
                        {
                            if (j < vecName[vecNum[vecIndex]].Length)
                                vecInfo.name[j] = vecName[vecNum[vecIndex]][j];
                            else
                                vecInfo.name[j] = ' ';
                        }
                        vecValue[domain] = new float[verDomainNum[domain]];
                        if (vecMax == vecMin)
                        {
                            for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                            {
                                vecValue[domain][verIndex] = 1;
                            }
                        }
                        else
                        {
                            for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                            {
                                vecValue[domain][verIndex] = (vectors[domain][verIndex] - vecMin) / (vecMax - vecMin);
                            }
                        }
                    }
                }
                

            }






        }
    }

}		
