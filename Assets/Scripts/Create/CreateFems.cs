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
using System.Threading.Tasks;
using System.Threading;

public class CreateFems : MonoBehaviour {
    public string[] inputFiles;
    private string[] inputFilePathes;
    private string inputFilePathDir;
    public string outputFile;
    string[] scaName = null,vecName = null;
    private List<int> scalarsList = new List<int>(), vectorsList = new List<int>();
    int[] scaNum = null,vecNum = null;
    private GameObject panelVar;//变量选择界面
    private Button ButtonOKSca;//变量选择界面的“确定”按钮
    private GameObject[] togglesSca, togglesVec;//变量选择界面的标量复选框、矢量复选框
    private GameObject panelGiv;//可视化界面
    private GameObject newObject;
    int timeNum;
    int domainNum;
    int inputScalarNum;
    int outputScalarNum;
    int charLength = 30;
    private GameObject TextSca, TextVec, TextSec;
    private GameObject dSection = null;
    private GameObject[] inpSection = null;
    private int numS = 0;
    int position = 0;

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
        CreateGivWithDomains();
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
        TextSca = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Text"));
        TextSca.GetComponent<Text>().text = "  Scalars:";
        TextSca.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        TextSca.transform.localPosition = new Vector3(-500, 350, 0);
        TextSca.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
        togglesSca = new GameObject[scaName.Length];
        for (int i = 0; i < scaName.Length; ++i)
        {
            if (i % 4 == 0)
                ++position;
            togglesSca[i] = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Toggle"));
            togglesSca[i].transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
            togglesSca[i].transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
            togglesSca[i].transform.localPosition = new Vector3(-500 + 300 * (i % 4), 350 - 40 * position, 0);
            togglesSca[i].GetComponentInChildren<Text>().text = scaName[i];
        }
        ++position;
        TextVec = (GameObject)Instantiate(Resources.Load("Prefabs/Widgets/Text"));
        TextVec.GetComponent<Text>().text = "  Vectors:";
        TextVec.transform.SetParent(GameObject.Find("Canvas/PanelVar").GetComponent<Transform>(), true);
        TextVec.transform.localPosition = new Vector3(-500, 350 - 40 * position, 0);
        TextVec.transform.localScale = GameObject.Find("Canvas/PanelVar").GetComponent<Transform>().localScale * 2;
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
        TextSec.GetComponent<Text>().text = "  ParallelNum:";
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
            if (togglesVec[i].GetComponent<Toggle>().isOn)
                vectorsList.Add(i);
        }
        vecNum = vectorsList.ToArray();
        panelVar.SetActive(false);
        BaseClass1(0,timeNum);

        panelGiv.SetActive(true);//可视化界面显示
        string[] readFiles = new string[timeNum];
        for (int t = 0; t < timeNum; ++t)
            readFiles[t] = outputFile.Replace(".giv", "") + t + ".giv";
        newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/ReadGiv"));
        newObject.GetComponent<ReadGiv>().files = readFiles;
        newObject.transform.parent = GetComponent<Transform>();
    }

    //Without parallel
    void BaseClass(int startTime,int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        int[] a = new int[5] { 1, 2, 4, 8, 16 };

        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        for(int domain = 0;domain<domainNum;++domain)
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    break;
                }
            }
            //读入节点
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    break;
                }
            }
            //读入单元--转化为三角形
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    break;
                }
            }

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "isExternal"))
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    break;
                }
            }
            //读入ghostRank
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "ghostRank"))
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    break;
                }
            }

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        }
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        for (int domain = 0; domain < domainNum; ++domain)
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                }
            }
        }

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                    List<string>[] listVtkForSca = new List<string>[domainNum];
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        if (verDomainNum[domain] != 0)
                        {
                            string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                            listVtkForSca[domain] = LoadTextFile(file);
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[verNum];
                                int scalarIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(scaName[scaNum[scaIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[scalarIndex] = float.Parse(lineToArray[i]);
                                                ++scalarIndex;
                                            }
                                            if (scalarIndex == verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                scalars[domain] = new float[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    scalars[domain][verIndex] = value[verDomainIndex[domain][verIndex]];
                                    scaMax = Mathf.Max(scalars[domain][verIndex], scaMax);
                                    scaMin = Mathf.Min(scalars[domain][verIndex], scaMin);
                                }

                            }
                        }
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
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
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                            verAdd = new Vector3[domainNum][];
                        //for (int domain = 0; domain < domainNum; ++domain)
                        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                Quaternion q = Quaternion.identity;
                                Vector3 v = new Vector3(0, 0, 1);
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[3 * verNum];
                                int vectorIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(vecName[vecNum[vecIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[vectorIndex] = float.Parse(lineToArray[i]);
                                                ++vectorIndex;
                                            }
                                            if (vectorIndex == 3 * verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                vectors[domain] = new float[verDomainNum[domain]];
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    q.SetFromToRotation(v, new Vector3(value[3 * verDomainIndex[domain][verIndex]],
                                    value[3 * verDomainIndex[domain][verIndex] + 1], value[3 * verDomainIndex[domain][verIndex] + 2]));
                                    vecQua[domain][verIndex] = q;
                                    vectors[domain][verIndex] = Mathf.Sqrt(value[3 * verDomainIndex[domain][verIndex]] * value[3 * verDomainIndex[domain][verIndex]]
                                        + value[3 * verDomainIndex[domain][verIndex] + 1] * value[3 * verDomainIndex[domain][verIndex] + 1]
                                        + value[3 * verDomainIndex[domain][verIndex] + 2] * value[3 * verDomainIndex[domain][verIndex] + 2]
                                        );
                                    vecMax = Mathf.Max(vectors[domain][verIndex], vecMax);
                                    vecMin = Mathf.Min(vectors[domain][verIndex], vecMin);
                                }
                                if (vecName[vecNum[vecIndex]] == "Displacement")
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        verAdd[domain][verIndex] = new Vector3(1000 * value[3 * verDomainIndex[domain][verIndex]] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 1] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 2] / (Max - Min));
                                    }
                                }
                            }
                        }
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        //Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                if (vecMax == vecMin)
                                {
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        vecValue[domain][verIndex] = 0.1f;
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
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }

    //domain for parallel
    void BaseClass1(int startTime, int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            for(int line1 = line;line1< listVtk.Count;++line1)
            {
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    break;
                }
            }
            //读入节点
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    break;
                }
            }
            //读入单元--转化为三角形
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    break;
                }
            }

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "isExternal"))
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    break;
                }
            }
            //读入ghostRank
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "ghostRank"))
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    break;
                }
            }

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        });
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                for(int verIndex = 0; verIndex < verDomainNum[domain];++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                }
            }
        });

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                    List<string>[] listVtkForSca = new List<string>[domainNum];
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        if (verDomainNum[domain] != 0)
                        {
                            string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                            listVtkForSca[domain] = LoadTextFile(file);
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[verNum];
                                int scalarIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(scaName[scaNum[scaIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[scalarIndex] = float.Parse(lineToArray[i]);
                                                ++scalarIndex;
                                            }
                                            if (scalarIndex == verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                scalars[domain] = new float[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    scalars[domain][verIndex] = value[verDomainIndex[domain][verIndex]];
                                    scaMax = Mathf.Max(scalars[domain][verIndex], scaMax);
                                    scaMin = Mathf.Min(scalars[domain][verIndex], scaMin);
                                }

                            }
                        });
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
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
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                            verAdd = new Vector3[domainNum][];
                        //for (int domain = 0; domain < domainNum; ++domain)
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                Quaternion q = Quaternion.identity;
                                Vector3 v = new Vector3(0, 0, 1);
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[3 * verNum];
                                int vectorIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(vecName[vecNum[vecIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[vectorIndex] = float.Parse(lineToArray[i]);
                                                ++vectorIndex;
                                            }
                                            if (vectorIndex == 3 * verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                vectors[domain] = new float[verDomainNum[domain]];
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    q.SetFromToRotation(v, new Vector3(value[3 * verDomainIndex[domain][verIndex]],
                                    value[3 * verDomainIndex[domain][verIndex] + 1], value[3 * verDomainIndex[domain][verIndex] + 2]));
                                    vecQua[domain][verIndex] = q;
                                    vectors[domain][verIndex] = Mathf.Sqrt(value[3 * verDomainIndex[domain][verIndex]] * value[3 * verDomainIndex[domain][verIndex]]
                                        + value[3 * verDomainIndex[domain][verIndex] + 1] * value[3 * verDomainIndex[domain][verIndex] + 1]
                                        + value[3 * verDomainIndex[domain][verIndex] + 2] * value[3 * verDomainIndex[domain][verIndex] + 2]
                                        );
                                    vecMax = Mathf.Max(vectors[domain][verIndex], vecMax);
                                    vecMin = Mathf.Min(vectors[domain][verIndex], vecMin);
                                }
                                if (vecName[vecNum[vecIndex]] == "Displacement")
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        verAdd[domain][verIndex] = new Vector3(1000 * value[3 * verDomainIndex[domain][verIndex]] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 1] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 2] / (Max - Min));
                                    }
                                }
                            }
                        });
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                if (vecMax == vecMin)
                                {
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        vecValue[domain][verIndex] = 0.1f;
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
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }
    void BaseClass1_(int startTime, int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    break;
                }
            }
            //读入节点
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    break;
                }
            }
            //读入单元--转化为三角形
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    break;
                }
            }

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "isExternal"))
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    break;
                }
            }
            //读入ghostRank
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "ghostRank"))
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    break;
                }
            }

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        });
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                }
            }
        });

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                    List<string>[] listVtkForSca = new List<string>[domainNum];
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        if (verDomainNum[domain] != 0)
                        {
                            string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                            listVtkForSca[domain] = LoadTextFile(file);
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[verNum];
                                int scalarIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(scaName[scaNum[scaIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[scalarIndex] = float.Parse(lineToArray[i]);
                                                ++scalarIndex;
                                            }
                                            if (scalarIndex == verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }

                            }
                        });
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                scaColor[domain] = new Color[verDomainNum[domain]];
                                //全为蓝色
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    scaColor[domain][verIndex] = Color.blue;
                                }
                            }
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                            verAdd = new Vector3[domainNum][];
                        //for (int domain = 0; domain < domainNum; ++domain)
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                Quaternion q = Quaternion.identity;
                                Vector3 v = new Vector3(0, 0, 1);
                                int verNum = 0;
                                int line = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[3 * verNum];
                                int vectorIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca[domain].Count; ++line1)
                                {
                                    if (listVtkForSca[domain][line1].Contains(vecName[vecNum[vecIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[vectorIndex] = float.Parse(lineToArray[i]);
                                                ++vectorIndex;
                                            }
                                            if (vectorIndex == 3 * verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    //全为q
                                    vecQua[domain][verIndex] = q;
                                }
                                if (vecName[vecNum[vecIndex]] == "Displacement")
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        verAdd[domain][verIndex] = Vector3.right;
                                    }
                                }
                            }
                        });
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                //全为0.1
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    vecValue[domain][verIndex] = 0.1f;
                                }
                            }
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }
    void BaseClass1__(int startTime, int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    break;
                }
            }
            //读入节点
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    break;
                }
            }
            //读入单元--转化为三角形
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    break;
                }
            }

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "isExternal"))
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    break;
                }
            }
            //读入ghostRank
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "ghostRank"))
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    break;
                }
            }

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        });
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                }
            }
        });

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                            }
                        });
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                scaColor[domain] = new Color[verDomainNum[domain]];
                                //全为蓝色
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    scaColor[domain][verIndex] = Color.blue;
                                }
                            }
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                        {
                            verAdd = new Vector3[domainNum][];
                            isVerAdd = true;
                            Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                            {
                                if (verDomainNum[domain] != 0)
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    //全为right
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        verAdd[domain][verIndex] = Vector3.right;
                                    }
                                }
                            });
                        }

                        //for (int domain = 0; domain < domainNum; ++domain)
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                            }
                        });
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        Quaternion q = Quaternion.identity;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                //全为0.1
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    vecQua[domain][verIndex] = q;
                                    vecValue[domain][verIndex] = 0.1f;
                                }
                            }
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }

    //domain for parallel（read vtk every scalar or vector）
    void BaseClass1_1(int startTime, int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], @"^avtOriginalBounds"))
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    break;
                }
            }
            //读入节点
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    break;
                }
            }
            //读入单元--转化为三角形
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], @"^CELLS"))
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    break;
                }
            }

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "isExternal"))
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    break;
                }
            }
            //读入ghostRank
            for (int line1 = line; line1 < listVtk.Count; ++line1)
            {
                //if (Regex.IsMatch(listVtk[line1], "ghostRank"))
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    break;
                }
            }

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        });
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                }
            }
        });

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                                List<string> listVtkForSca = new List<string>();
                                string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                                listVtkForSca = LoadTextFile(file);
                                for (int line1 = line; line1 < listVtkForSca.Count; ++line1)
                                {
                                    if (listVtkForSca[line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[verNum];
                                int scalarIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca.Count; ++line1)
                                {
                                    if (listVtkForSca[line1].Contains(scaName[scaNum[scaIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca.Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[scalarIndex] = float.Parse(lineToArray[i]);
                                                ++scalarIndex;
                                            }
                                            if (scalarIndex == verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                scalars[domain] = new float[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    scalars[domain][verIndex] = value[verDomainIndex[domain][verIndex]];
                                    scaMax = Mathf.Max(scalars[domain][verIndex], scaMax);
                                    scaMin = Mathf.Min(scalars[domain][verIndex], scaMin);
                                }

                            }
                        });
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
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
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                            verAdd = new Vector3[domainNum][];
                        //for (int domain = 0; domain < domainNum; ++domain)
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                Quaternion q = Quaternion.identity;
                                Vector3 v = new Vector3(0, 0, 1);
                                int verNum = 0;
                                int line = 0;
                                DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                                List<string> listVtkForSca = new List<string>();
                                string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                                listVtkForSca = LoadTextFile(file);
                                for (int line1 = line; line1 < listVtkForSca.Count; ++line1)
                                {
                                    if (listVtkForSca[line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        break;
                                    }
                                }
                                float[] value = new float[3 * verNum];
                                int vectorIndex = 0;
                                for (int line1 = line; line1 < listVtkForSca.Count; ++line1)
                                {
                                    if (listVtkForSca[line1].Contains(vecName[vecNum[vecIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca.Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[vectorIndex] = float.Parse(lineToArray[i]);
                                                ++vectorIndex;
                                            }
                                            if (vectorIndex == 3 * verNum)
                                                break;
                                        }
                                        break;
                                    }
                                }
                                vectors[domain] = new float[verDomainNum[domain]];
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                {
                                    q.SetFromToRotation(v, new Vector3(value[3 * verDomainIndex[domain][verIndex]],
                                    value[3 * verDomainIndex[domain][verIndex] + 1], value[3 * verDomainIndex[domain][verIndex] + 2]));
                                    vecQua[domain][verIndex] = q;
                                    vectors[domain][verIndex] = Mathf.Sqrt(value[3 * verDomainIndex[domain][verIndex]] * value[3 * verDomainIndex[domain][verIndex]]
                                        + value[3 * verDomainIndex[domain][verIndex] + 1] * value[3 * verDomainIndex[domain][verIndex] + 1]
                                        + value[3 * verDomainIndex[domain][verIndex] + 2] * value[3 * verDomainIndex[domain][verIndex] + 2]
                                        );
                                    vecMax = Mathf.Max(vectors[domain][verIndex], vecMax);
                                    vecMin = Mathf.Min(vectors[domain][verIndex], vecMin);
                                }
                                if (vecName[vecNum[vecIndex]] == "Displacement")
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        verAdd[domain][verIndex] = new Vector3(1000 * value[3 * verDomainIndex[domain][verIndex]] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 1] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 2] / (Max - Min));
                                    }
                                }
                            }
                        });
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        Parallel.For(0, domainNum, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (domain) =>
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                if (vecMax == vecMin)
                                {
                                    for (int verIndex = 0; verIndex < verDomainNum[domain]; ++verIndex)
                                    {
                                        vecValue[domain][verIndex] = 0.1f;
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
                        });
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }

    //vertice for parallel
    void BaseClass2(int startTime, int endTime)
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

        //范围、节点Index、三角形Index
        int[] verDomainNum, triDomainNum;
        int[][] verDomainIndex, triDomain;
        float[][] points;
        int[][] triangles;
        verDomainNum = new int[domainNum];
        triDomainNum = new int[domainNum];
        verDomainIndex = new int[domainNum][];
        triDomain = new int[domainNum][];
        points = new float[domainNum][];
        triangles = new int[domainNum][];

        Vector3[][] vertices = null;

        bool changeMesh = false;
        Vector3 maxVertice, minVertice;
        bool isVerAdd = false;
        int scalarsNum = scaNum.Length,
        vectorsNum = vecNum.Length;
        float xMax = float.MinValue, xMin = float.MaxValue,
            yMax = float.MinValue, yMin = float.MaxValue,
            zMax = float.MinValue, zMin = float.MaxValue;
        float Max = 0, Min = 0;
        Quaternion q = Quaternion.identity;
        Vector3 v = new Vector3(0, 0, 1);
        int[] a = new int[5] { 1, 2, 4, 8, 16 };
        int parallelNum = a[dSection.transform.GetComponent<Dropdown>().value];
        for (int domain = 0; domain < domainNum; ++domain)
        {
            List<string> listVtk = LoadTextFile(inputFiles[domain]);
            int line = 0;
            int verNum = 0, triNum = 0;
            float[] isExternal = null, ghostRank = null;
            //x,y,z的范围
            Parallel.For(line, listVtk.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
            {
                if (listVtk[line1].Contains("avtOriginalBounds "))
                {
                    string[] lineToArray = listVtk[line1 + 1].Split(' ');
                    xMin = Mathf.Min(xMin, float.Parse(lineToArray[0]));
                    xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                    yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                    yMax = Mathf.Max(yMax, float.Parse(lineToArray[3]));
                    zMin = Mathf.Min(zMin, float.Parse(lineToArray[4]));
                    zMax = Mathf.Max(zMax, float.Parse(lineToArray[5]));
                    line = line1;
                    ParallelLoopState.Stop();
                    return;
                }
            });
            //读入节点
            Parallel.For(line, listVtk.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
            {
                if (listVtk[line1].Contains("POINTS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line1;
                    points[domain] = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            points[domain][pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                    line = line1;
                    ParallelLoopState.Stop();
                    return;
                }
            });
            //读入单元--转化为三角形
            Parallel.For(line, listVtk.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
            {
                if (listVtk[line1].Contains("CELLS "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    int cellNum = int.Parse(lineToArray[1]);
                    ++line1;
                    if (int.Parse(listVtk[line1].Split(' ')[0]) == 4)//编号4，则为四面体单元，可拓展
                    {
                        triNum = 4 * cellNum;
                        triangles[domain] = new int[3 * triNum];
                        int triIndex = 0;
                        for (; line1 < listVtk.Count; ++line1)
                        {
                            lineToArray = listVtk[line1].Split(' ');
                            triangles[domain][triIndex] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 1] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 2] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 3] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 4] = int.Parse(lineToArray[3]);
                            triangles[domain][triIndex + 5] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 6] = int.Parse(lineToArray[1]);
                            triangles[domain][triIndex + 7] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 8] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 9] = int.Parse(lineToArray[2]);
                            triangles[domain][triIndex + 10] = int.Parse(lineToArray[4]);
                            triangles[domain][triIndex + 11] = int.Parse(lineToArray[3]);
                            triIndex += 12;
                            if (triIndex == 3 * triNum)
                                break;
                        }
                    }
                    line = line1;
                    ParallelLoopState.Stop();
                    return;
                }
            });

            isExternal = new float[verNum];
            SetArrayValue(isExternal, 1);
            ghostRank = new float[verNum];
            SetArrayValue(ghostRank, -1);
            //读入isExternal
            Parallel.For(line, listVtk.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
            {
                if (listVtk[line1].Contains("isExternal "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int isExternalIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            isExternal[isExternalIndex] = float.Parse(lineToArray[i]);
                            ++isExternalIndex;
                        }
                        if (isExternalIndex == verNum)
                            break;
                    }
                    ParallelLoopState.Stop();
                    return;
                }
            });
            //读入ghostRank
            Parallel.For(line, listVtk.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
            {
                if (listVtk[line1].Contains("ghostRank "))
                {
                    string[] lineToArray = listVtk[line1].Split(' ');
                    ++line1;
                    if (listVtk[line1].Split(' ')[1] == "default")
                        ++line1;
                    int ghostRankIndex = 0;
                    for (; line1 < listVtk.Count; ++line1)
                    {
                        lineToArray = listVtk[line1].Split(' ');
                        for (int i = 0; i < lineToArray.Length - 1; ++i)
                        {
                            ghostRank[ghostRankIndex] = float.Parse(lineToArray[i]);
                            ++ghostRankIndex;
                        }
                        if (ghostRankIndex == verNum)
                            break;
                    }
                    ParallelLoopState.Stop();
                    return;
                }
            });

            List<int> arrayTri = new List<int>();
            List<int> arrayVerIndex = new List<int>();
            int[] markVer = new int[verNum];
            int verNumNew = 1;
            //根据IsExternal和ghostRank挑选外部的节点和单元
            for (int triIndex = 0; triIndex < triNum; ++triIndex)
            {
                if (isExternal[triangles[domain][3 * triIndex]] == 1
                && isExternal[triangles[domain][3 * triIndex + 1]] == 1
                && isExternal[triangles[domain][3 * triIndex + 2]] == 1)
                {
                    if (ghostRank[triangles[domain][3 * triIndex]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 1]] < 0
                    || ghostRank[triangles[domain][3 * triIndex + 2]] < 0)
                    {
                        if (ghostRank[triangles[domain][3 * triIndex]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 1]] < domain
                        && ghostRank[triangles[domain][3 * triIndex + 2]] < domain)
                        {
                            if (markVer[triangles[domain][3 * triIndex]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex]);
                                markVer[triangles[domain][3 * triIndex]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 1]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 1]);
                                markVer[triangles[domain][3 * triIndex + 1]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 1]] - 1);
                            if (markVer[triangles[domain][3 * triIndex + 2]] == 0)
                            {
                                arrayVerIndex.Add(triangles[domain][3 * triIndex + 2]);
                                markVer[triangles[domain][3 * triIndex + 2]] = verNumNew;
                                arrayTri.Add(verNumNew - 1);
                                ++verNumNew;
                            }
                            else
                                arrayTri.Add(markVer[triangles[domain][3 * triIndex + 2]] - 1);
                        }
                    }
                }
            }
            verDomainNum[domain] = verNumNew - 1;
            triDomain[domain] = arrayTri.ToArray();
            triDomainNum[domain] = triDomain[domain].Length / 3;
            verDomainIndex[domain] = arrayVerIndex.ToArray();
        }
        maxVertice = new Vector3(xMax, yMax, zMax);
        minVertice = new Vector3(xMin, yMin, zMin);
        Max = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMax : zMax) : (yMax - yMin > zMax - zMin ? yMax : zMax);
        Min = xMax - xMin > yMax - yMin ? (xMax - xMin > zMax - zMin ? xMin : zMin) : (yMax - yMin > zMax - zMin ? yMin : zMin);

        vertices = new Vector3[domainNum][];
        for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
        {
            if (vecName[vecNum[vecIndex]] == "Displacement")
                isVerAdd = true;
        }
        sumSize += sizeV * 2 + (sizeChar * charLength + sizeF * 2) * (scalarsNum + vectorsNum);
        for (int domain = 0; domain < domainNum; ++domain)
        {
            sumSize += sizeI * 2;
            if (verDomainNum[domain] != 0)
                sumSize += (sizeV + sizeC * scalarsNum + (sizeQ + sizeF) * vectorsNum) * verDomainNum[domain]
                    + sizeI * triDomainNum[domain] * 3;
            if (isVerAdd)
                sumSize += sizeV * verDomainNum[domain];
        }

        //节点归一化
        for (int domain = 0; domain < domainNum; ++domain)
        {
            if (verDomainNum[domain] != 0)
            {
                vertices[domain] = new Vector3[verDomainNum[domain]];
                float xx = (xMax + xMin) / 2, yy = (yMax + yMin) / 2, zz = (zMax + zMin) / 2;
                Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                {
                    vertices[domain][verIndex] = new Vector3(
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex]] - xx) / (Max - Min),//
                        -10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 1] - yy) / (Max - Min),
                        10 * (points[domain][3 * verDomainIndex[domain][verIndex] + 2] - zz) / (Max - Min));
                });
            }
        }

        for (int time = startTime; time < endTime; ++time)
        {
            string outputFiles = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            CreateFileWithSize(outputFiles, sumSize);
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles))
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write<bool>(jf, ref changeMesh);
                    jf += sizeB;
                    accessor.Write<int>(jf, ref domainNum);
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
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        accessor.Write<int>(jf, ref verDomainNum[domain]);
                        jf += sizeI;
                        int triVerNum = 3 * triDomainNum[domain];
                        accessor.Write<int>(jf, ref triVerNum);
                        jf += sizeI;
                        if (verDomainNum[domain] != 0)
                        {
                            accessor.WriteArray<Vector3>(jf, vertices[domain], 0, verDomainNum[domain]);
                            jf += sizeV * verDomainNum[domain];
                            accessor.WriteArray<int>(jf, triDomain[domain], 0, triVerNum);
                            jf += sizeI * triVerNum;
                        }
                    }
                    DirectoryInfo folder = new DirectoryInfo(inputFilePathes[time]);
                    List<string>[] listVtkForSca = new List<string>[domainNum];
                    for (int domain = 0; domain < domainNum; ++domain)
                    {
                        if (verDomainNum[domain] != 0)
                        {
                            string file = folder.GetFiles("*." + domain + ".vtk")[0].FullName;
                            listVtkForSca[domain] = LoadTextFile(file);
                        }
                    }
                    //标量
                    for (int scaIndex = 0; scaIndex < scalarsNum; ++scaIndex)
                    {
                        float[][] scalars = new float[domainNum][];
                        float scaMax = float.MinValue, scaMin = float.MaxValue;
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                Parallel.For(line, listVtkForSca[domain].Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
                                {
                                    //if (Regex.IsMatch(listVtkForSca[domain][line1], @"^POINTS"))
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        ParallelLoopState.Stop();
                                        return;
                                    }
                                });
                                float[] value = new float[verNum];
                                int scalarIndex = 0;
                                Parallel.For(line, listVtkForSca[domain].Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
                                {
                                    //if (Regex.IsMatch(listVtkForSca[domain][line1], scaName[scaNum[scaIndex]] + " "))
                                    if (listVtkForSca[domain][line1].Contains(scaName[scaNum[scaIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[scalarIndex] = float.Parse(lineToArray[i]);
                                                ++scalarIndex;
                                            }
                                            if (scalarIndex == verNum)
                                                break;
                                        }
                                        //line = line1;
                                        ParallelLoopState.Stop();
                                        return;
                                    }
                                });
                                scalars[domain] = new float[verDomainNum[domain]];
                                Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                {
                                    scalars[domain][verIndex] = value[verDomainIndex[domain][verIndex]];
                                    scaMax = Mathf.Max(scalars[domain][verIndex], scaMax);
                                    scaMin = Mathf.Min(scalars[domain][verIndex], scaMin);
                                });

                            }
                        }
                        info scaInfo;
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
                        accessor.WriteArray<char>(jf, scaInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref scaInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref scaInfo.min);
                        jf += sizeF;
                        Color[][] scaColor;
                        scaColor = new Color[domainNum][];
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                scaColor[domain] = new Color[verDomainNum[domain]];
                                if (scaMax == scaMin)
                                {
                                    Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                    {
                                        scaColor[domain][verIndex] = Color.blue;
                                    });
                                }
                                else
                                {
                                    Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                    {
                                        scaColor[domain][verIndex] = FourColor((scalars[domain][verIndex] - scaMin) / (scaMax - scaMin));
                                    });
                                }
                            }
                        }
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Color>(jf, scaColor[domain], 0, verDomainNum[domain]);
                                jf += sizeC * verDomainNum[domain];
                            }
                        }
                    }

                    Vector3[][] verAdd = null;
                    //矢量
                    for (int vecIndex = 0; vecIndex < vectorsNum; ++vecIndex)
                    {
                        float[][] vectors = new float[domainNum][];
                        Quaternion[][] vecQua;
                        float[][] vecValue;
                        vecValue = new float[domainNum][];
                        vecQua = new Quaternion[domainNum][];
                        float vecMax = float.MinValue, vecMin = float.MaxValue;
                        if (vecName[vecNum[vecIndex]] == "Displacement")
                            verAdd = new Vector3[domainNum][];
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                int verNum = 0;
                                int line = 0;
                                Parallel.For(line, listVtkForSca[domain].Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
                                {
                                    if (listVtkForSca[domain][line1].Contains("POINTS "))
                                    {
                                        string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                        verNum = int.Parse(lineToArray[1]);
                                        line = line1;
                                        ParallelLoopState.Stop();
                                        return;
                                    }
                                });
                                float[] value = new float[3 * verNum];
                                int vectorIndex = 0;
                                Parallel.For(line, listVtkForSca[domain].Count, new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (line1, ParallelLoopState) =>
                                {
                                    if (listVtkForSca[domain][line1].Contains(vecName[vecNum[vecIndex]] + " "))
                                    {
                                        ++line1;
                                        if (listVtkForSca[domain][line1].Split(' ')[1] == "default")
                                            ++line1;
                                        for (; line1 < listVtkForSca[domain].Count; ++line1)
                                        {
                                            string[] lineToArray = listVtkForSca[domain][line1].Split(' ');
                                            for (int i = 0; i < lineToArray.Length - 1; ++i)
                                            {
                                                value[vectorIndex] = float.Parse(lineToArray[i]);
                                                ++vectorIndex;
                                            }
                                            if (vectorIndex == 3 * verNum)
                                                break;
                                        }
                                        ParallelLoopState.Stop();
                                        return;
                                    }
                                });
                                vectors[domain] = new float[verDomainNum[domain]];
                                vecQua[domain] = new Quaternion[verDomainNum[domain]];
                                Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                {
                                    q.SetFromToRotation(v, new Vector3(value[3 * verDomainIndex[domain][verIndex]],
                                    value[3 * verDomainIndex[domain][verIndex] + 1], value[3 * verDomainIndex[domain][verIndex] + 2]));
                                    vecQua[domain][verIndex] = q;
                                    vectors[domain][verIndex] = Mathf.Sqrt(value[3 * verDomainIndex[domain][verIndex]] * value[3 * verDomainIndex[domain][verIndex]]
                                        + value[3 * verDomainIndex[domain][verIndex] + 1] * value[3 * verDomainIndex[domain][verIndex] + 1]
                                        + value[3 * verDomainIndex[domain][verIndex] + 2] * value[3 * verDomainIndex[domain][verIndex] + 2]
                                        );
                                    vecMax = Mathf.Max(vectors[domain][verIndex], vecMax);
                                    vecMin = Mathf.Min(vectors[domain][verIndex], vecMin);
                                });
                                if (vecName[vecNum[vecIndex]] == "Displacement")
                                {
                                    verAdd[domain] = new Vector3[verDomainNum[domain]];
                                    Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                    {
                                        verAdd[domain][verIndex] = new Vector3(1000 * value[3 * verDomainIndex[domain][verIndex]] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 1] / (Max - Min),
                                            1000 * value[3 * verDomainIndex[domain][verIndex] + 2] / (Max - Min));
                                    });
                                }
                            }
                        }
                        info vecInfo;
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
                        accessor.WriteArray<char>(jf, vecInfo.name, 0, charLength);
                        jf += sizeChar * charLength;
                        accessor.Write<float>(jf, ref vecInfo.max);
                        jf += sizeF;
                        accessor.Write<float>(jf, ref vecInfo.min);
                        jf += sizeF;
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                vecValue[domain] = new float[verDomainNum[domain]];
                                if (vecMax == vecMin)
                                {
                                    Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                    {
                                        vecValue[domain][verIndex] = 0.1f;
                                    });
                                }
                                else
                                {
                                    Parallel.For(0, verDomainNum[domain], new ParallelOptions { MaxDegreeOfParallelism = parallelNum }, (verIndex) =>
                                    {
                                        vecValue[domain][verIndex] = (vectors[domain][verIndex] - vecMin) / (vecMax - vecMin);
                                    });
                                }
                            }
                        }
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Quaternion>(jf, vecQua[domain], 0, verDomainNum[domain]);
                                jf += sizeQ * verDomainNum[domain];
                                accessor.WriteArray<float>(jf, vecValue[domain], 0, verDomainNum[domain]);
                                jf += sizeF * verDomainNum[domain];
                            }
                        }
                    }
                    if (isVerAdd)
                    {
                        for (int domain = 0; domain < domainNum; ++domain)
                        {
                            if (verDomainNum[domain] != 0)
                            {
                                accessor.WriteArray<Vector3>(jf, verAdd[domain], 0, verDomainNum[domain]);
                                jf += sizeV * verDomainNum[domain];
                            }
                        }
                    }
                }
            }
        }
    }
}	
