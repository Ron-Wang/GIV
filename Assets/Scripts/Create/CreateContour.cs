using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System;
public class CreateContour : MonoBehaviour {
    public string[] inputFiles;
    public string outputFile;
    private string[] outputFiles;
    int timeNum;
    int inputScalarNum;
    int outputScalarNum;
    private GameObject panelGiv;//可视化界面
    private GameObject newObject;

    //读取.vtk文件或.obj文件(则无值)
    void Start () {
        panelGiv = GameObject.Find("Canvas/PanelGiv");
        panelGiv.SetActive(false);
        timeNum = inputFiles.Length;
        outputFiles = new string[timeNum];
        CreateGivByObj();
    }

    public void CreateGivByVtk()
    {
    }
    public void CreateGivByObj()//obj文件的读取、归一化、写入giv文件
    {
        List<string> listObj;
        float Max = 0, Min = 0;
        float xMax = 0, xMin = 0,
            yMax = 0, yMin = 0,
            zMax = 0, zMin = 0;
        int verNum, triNum;
        bool changeMesh = true;
        long sizeB = Marshal.SizeOf(typeof(bool));
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeF = sizeof(float);
        long sumSize;

        for (int time = 0;time < timeNum; ++time)
        {
            sumSize = sizeB + sizeI + sizeV * 2;
            listObj = LoadTextFile(inputFiles[time]);
            verNum = 0;
            triNum = 0;
            List<float> listVer = new List<float>();
            List<int> listTri = new List<int>();
            for (int line = 0; line < listObj.Count; ++line)
            {
                string[] lineToArray = listObj[line].Split(' ');
                if (lineToArray[0] == "v")
                {
                    listVer.Add(float.Parse(lineToArray[1]));
                    listVer.Add(float.Parse(lineToArray[2]));
                    listVer.Add(float.Parse(lineToArray[3]));
                    //寻坐标最值
                    if(time == 0)
                    {
                        if (verNum == 0)
                        {
                            xMax = float.Parse(lineToArray[1]);
                            yMax = float.Parse(lineToArray[2]);
                            zMax = float.Parse(lineToArray[3]);
                            xMin = xMax;
                            yMin = yMax;
                            zMin = zMax;
                        }
                        else
                        {
                            xMax = Mathf.Max(xMax, float.Parse(lineToArray[1]));
                            yMax = Mathf.Max(yMax, float.Parse(lineToArray[2]));
                            zMax = Mathf.Max(zMax, float.Parse(lineToArray[3]));
                            xMin = Mathf.Min(xMin, float.Parse(lineToArray[1]));
                            yMin = Mathf.Min(yMin, float.Parse(lineToArray[2]));
                            zMin = Mathf.Min(zMin, float.Parse(lineToArray[3]));
                        }
                    }
                    ++verNum;
                }
                if (lineToArray[0] == "f")
                {
                    
                    listTri.Add(int.Parse(lineToArray[1].Split('/')[0]) - 1);
                    listTri.Add(int.Parse(lineToArray[3].Split('/')[0]) - 1);
                    listTri.Add(int.Parse(lineToArray[2].Split('/')[0]) - 1);
                    /*                    
                    listTri.Add(int.Parse(lineToArray[1]) - 1);
                    listTri.Add(int.Parse(lineToArray[3]) - 1);
                    listTri.Add(int.Parse(lineToArray[2]) - 1);
                    */
                    ++triNum;
                }
            }

            if (xMax - xMin > yMax - yMin)
            {
                if (xMax - xMin > zMax - zMin)
                {
                    Max = xMax;
                    Min = xMin;
                }
                else
                {
                    Max = zMax;
                    Min = zMin;
                }
            }
            else
            {
                if (yMax - yMin > zMax - zMin)
                {
                    Max = yMax;
                    Min = yMin;
                }
                else
                {
                    Max = zMax;
                    Min = zMin;
                }
            }

            float[] points = listVer.ToArray();
            int[] triangles = listTri.ToArray();
            Vector3[] vertices = new Vector3[verNum];
            Vector3 maxVertice = new Vector3(xMax,yMax,zMax),
                minVertice = new Vector3(xMin, yMin, zMin);
            for (int iVer = 0; iVer < verNum; ++iVer)//节点归一化
            {
                vertices[iVer] = new Vector3(10 * (points[3 * iVer] - xMin) / (Max - Min) - 5,
                    5 - 10 * (points[3 * iVer + 1] - yMin) / (Max - Min),
                    10 * (points[3 * iVer + 2] - zMin) / (Max - Min) - 5);
            }
            //60000节点的判断、分片
            int meshNum, maxVerNum = 60000;
            outputFiles[time] = outputFile.Replace(".giv", "") + time + ".giv";
            long jf = 0;
            if (verNum <= maxVerNum)//不足60000节点，无需划分
            {
                meshNum = 1;
                sumSize += sizeI * 2 + sizeV * verNum + 3 * sizeI * triNum + sizeF;
                CreateFileWithSize(outputFiles[time], sumSize);
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[time]))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        accessor.Write<bool>(jf, ref changeMesh);
                        jf += sizeB;
                        accessor.Write<int>(jf, ref meshNum);
                        jf += sizeI;
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
                        float value = 0;
                        accessor.Write<float>(jf, ref value);
                    }
                }
            }
            else//超过60000节点，需要划分
            {
                meshNum = 0;
                int triRemain = triNum,//剩余单元数
                    triIndex = 0;
                int verMax;
                bool isExist;
                int[] trianglesNew = new int[3 * triNum];
                List<int> arrayListTriNum = new List<int>(),//每个网格单元数
                    arrayListVerNum = new List<int>(),//每个网格节点数
                    arrayListVerNew = new List<int>();//所有网格节点在原网格的不重复编号
                int i = 0, j, k;
                while (triRemain > 0)
                {
                    verMax = 0;
                    int[] markExist = new int[verNum];
                    while (verMax < maxVerNum)
                    {
                        for (k = 0; k < 3; ++k)
                        {
                            if (markExist[triangles[3 * i + k]] == 0)
                            {
                                arrayListVerNew.Add(triangles[3 * i + k]);
                                trianglesNew[3 * i + k] = verMax;
                                ++verMax;
                                markExist[triangles[3 * i + k]] = verMax;
                            }
                            else
                            {
                                trianglesNew[3 * i + k] = markExist[triangles[3 * i + k]] - 1;
                            }

                            /*
                            if (i == triIndex && k == 0)//每个网格的第一个节点直接编号
                            {
                                arrayListVerNew.Add(triangles[3 * i]);
                                trianglesNew[3 * i] = 0;
                                ++verMax;
                            }
                            else//非第一个节点，则需要遍历之前是否出现过
                            {
                                isExist = true;
                                for (j = 3 * triIndex; j < 3 * i + k; ++j)
                                {
                                    if(triangles[3 * i + k] == triangles[j])//出现
                                    {
                                        trianglesNew[3 * i + k] = trianglesNew[j];
                                        isExist = false;
                                        break;
                                    }
                                }
                                if (isExist)//未出现，则增加新编号
                                {
                                    arrayListVerNew.Add(triangles[3 * i + k]);
                                    trianglesNew[3 * i + k] = verMax;
                                    ++verMax;
                                }
                            }
                            */


                        }
                        ++i;
                        if (i == triNum)
                            break;
                    }
                    arrayListTriNum.Add(i - triIndex);
                    arrayListVerNum.Add(verMax);
                    triIndex = i;
                    ++meshNum;
                    triRemain = triNum - triIndex;
                }
                int[] triNumNew = arrayListTriNum.ToArray(),
                    verNumNew = arrayListVerNum.ToArray(); 
                int[] verNew = arrayListVerNew.ToArray();
                Vector3[][] verticesNew = new Vector3[meshNum][];
                int verNewIndex = 0, ii;
                for (int meshIndex = 0; meshIndex < meshNum; ++meshIndex)
                {
                    sumSize += sizeI * 2 + sizeV * verNumNew[meshIndex] + 3 * sizeI * triNumNew[meshIndex] + sizeF;
                    verticesNew[meshIndex] = new Vector3[verNumNew[meshIndex]];
                    for (ii = 0; ii < verNumNew[meshIndex]; ++ii)
                    {
                        verticesNew[meshIndex][ii] = vertices[verNew[verNewIndex + ii]];
                    }
                    verNewIndex += verNumNew[meshIndex];
                }
                CreateFileWithSize(outputFiles[time], sumSize);
                jf = 0;
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[time]))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        accessor.Write<bool>(jf, ref changeMesh);
                        jf += sizeB;
                        accessor.Write<int>(jf, ref meshNum);
                        jf += sizeI;
                        accessor.Write<Vector3>(jf, ref maxVertice);
                        jf += sizeV;
                        accessor.Write<Vector3>(jf, ref minVertice);
                        jf += sizeV;
                        int triIndexNew = 0;
                        for (int meshIndex = 0; meshIndex < meshNum; ++meshIndex)
                        {
                            accessor.Write<int>(jf, ref verNumNew[meshIndex]);
                            jf += sizeI;
                            int triVerNum = 3 * triNumNew[meshIndex];
                            accessor.Write<int>(jf, ref triVerNum);
                            jf += sizeI;
                            accessor.WriteArray<Vector3>(jf, verticesNew[meshIndex], 0, verNumNew[meshIndex]);
                            jf += sizeV * verNumNew[meshIndex];
                            accessor.WriteArray<int>(jf, trianglesNew, triIndexNew, triVerNum);
                            jf += sizeI * triVerNum;
                            triIndexNew += triVerNum;
                            float value = 0;
                            accessor.Write<float>(jf, ref value);
                            jf += sizeF;
                        }
                    }
                }
            }
        }
        panelGiv.SetActive(true);//可视化界面显示
        newObject = (GameObject)Instantiate(Resources.Load("Prefabs/ReadGiv"));
        newObject.GetComponent<ReadGiv>().files = outputFiles;
        newObject.transform.parent = GetComponent<Transform>();
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
}
