using System;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.RegularExpressions;

public class CreateBg : MonoBehaviour {
    public string[] BgPath;
    public float Max, Min, xMin, yMin, zMin;
    public string outputFileDir;

	// Use this for initialization
	void Start () {

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

    public void CreateGivb(){
        List<string> listVtk;
        int verNum, triNum;
        long sizeI = sizeof(int);
        long sizeV = Marshal.SizeOf(typeof(Vector3));
        long sizeF = sizeof(float);
        long sumSize = 0;
        string[] outputFiles = new string[BgPath.Length];

        for (int Bgi = 0; Bgi < BgPath.Length; ++Bgi)
        {
            sumSize = sizeI;
            listVtk = LoadTextFile(BgPath[Bgi]);
            verNum = 0;
            triNum = 0;
            float[] points = null;
            int[] cell = null;
            int[] triangles = null;
            int cellNum = 0;
            int cellType = 0;
            for (int line = 0; line < listVtk.Count; ++line)
            {
                /*
                string[] lineToArray = listObj[line].Split(' ');
                if (lineToArray[0] == "v")
                {
                    listVer.Add(float.Parse(lineToArray[1]));
                    listVer.Add(float.Parse(lineToArray[2]));
                    listVer.Add(float.Parse(lineToArray[3]));
                    ++verNum;
                }
                if (lineToArray[0] == "f")
                {

                    listTri.Add(int.Parse(lineToArray[1].Split('/')[0]) - 1);
                    listTri.Add(int.Parse(lineToArray[3].Split('/')[0]) - 1);
                    listTri.Add(int.Parse(lineToArray[2].Split('/')[0]) - 1);
                    ++triNum;
                }
                */
                //节点
                if (Regex.IsMatch(listVtk[line], @"^POINTS"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    verNum = int.Parse(lineToArray[1]);
                    ++line;
                    points = new float[3 * verNum];
                    int pointIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        for (int i = 0; i < lineToArray.Length; ++i)
                        {
                            points[pointIndex] = float.Parse(lineToArray[i]);
                            ++pointIndex;
                        }
                        if (pointIndex == 3 * verNum)
                            break;
                    }
                }
                //单元
                if (Regex.IsMatch(listVtk[line], @"^CELLS"))
                {
                    string[] lineToArray = listVtk[line].Split(' ');
                    cellNum = int.Parse(lineToArray[1]);
                    ++line;
                    int pointInCell = int.Parse(listVtk[line].Split(' ')[0]);
                    cell = new int[pointInCell * cellNum];
                    int cellIndex = 0;
                    for (; line < listVtk.Count; ++line)
                    {
                        lineToArray = listVtk[line].Split(' ');
                        for(int i = 0;i<pointInCell;++i)
                            cell[cellIndex + i] = int.Parse(lineToArray[i + 1]);
                        cellIndex += pointInCell;
                        if (cellIndex == pointInCell * cellNum)
                            break;
                    }
                }
                if (Regex.IsMatch(listVtk[line], @"^CELL_TYPES"))
                {
                    ++line;
                    cellType = int.Parse(listVtk[line].Split(' ')[0]);
                    if (cellType == 10)//四面体
                    {
                        triNum = 4 * cellNum;
                        triangles = new int[3 * triNum];
                        for (int i = 0; i < cellNum; ++i)
                        {
                            triangles[12 * i] = cell[4 * i];
                            triangles[12 * i + 1] = cell[4 * i + 1];
                            triangles[12 * i + 2] = cell[4 * i + 2];
                            triangles[12 * i + 3] = cell[4 * i];
                            triangles[12 * i + 4] = cell[4 * i + 2];
                            triangles[12 * i + 5] = cell[4 * i + 3];
                            triangles[12 * i + 6] = cell[4 * i];
                            triangles[12 * i + 7] = cell[4 * i + 3];
                            triangles[12 * i + 8] = cell[4 * i + 1];
                            triangles[12 * i + 9] = cell[4 * i + 1];
                            triangles[12 * i + 10] = cell[4 * i + 3];
                            triangles[12 * i + 11] = cell[4 * i + 2];
                        }
                    }
                    else if (cellType == 9)//四边形
                    {
                        triNum = 2 * cellNum;
                        triangles = new int[3 * triNum];
                        for (int i = 0; i < cellNum; ++i)
                        {
                            triangles[6 * i] = cell[4 * i];
                            triangles[6 * i + 1] = cell[4 * i + 1];
                            triangles[6 * i + 2] = cell[4 * i + 2];
                            triangles[6 * i + 3] = cell[4 * i];
                            triangles[6 * i + 4] = cell[4 * i + 2];
                            triangles[6 * i + 5] = cell[4 * i + 3];
                        }
                    }
                    else//暂代两点线段，可拓展
                    {
                        triNum = 2 * cellNum;
                        triangles = new int[3 * triNum];
                        for (int i = 0; i < cellNum; ++i)
                        {
                            int cellNow = cell[2 * i] / 2;
                            triangles[6 * i] = 4 * cellNow;
                            triangles[6 * i + 1] = 4 * cellNow + 1;
                            triangles[6 * i + 2] = 4 * cellNow + 2;
                            triangles[6 * i + 3] = 4 * cellNow;
                            triangles[6 * i + 4] = 4 * cellNow + 2;
                            triangles[6 * i + 5] = 4 * cellNow + 3;
                        }

                    }
                    break;
                }
            }
            Vector3[] vertices;
            if (cellType != 3)
            {
                vertices = new Vector3[verNum];
                for (int iVer = 0; iVer < verNum; ++iVer)//节点归一化
                {
                    vertices[iVer] = new Vector3(10 * (points[3 * iVer] - xMin) / (Max - Min) - 5,
                        5 - 10 * (points[3 * iVer + 1] - yMin) / (Max - Min),
                        10 * (points[3 * iVer + 2] - zMin) / (Max - Min) - 5);
                }
            }
            else
            {
                verNum *= 2;
                vertices = new Vector3[verNum];
                Vector3 delta;//线段长度？？？？？
                float f = 0.1f;
                for (int iCell = 0; iCell < cellNum; ++iCell)//节点归一化
                {
                    if (points[6 * iCell] != points[6 * iCell + 3])
                    {
                        float x = ((points[6 * iCell + 4] - points[6 * iCell + 1])
                            + (points[6 * iCell + 5] - points[6 * iCell + 2]))
                            / (points[6 * iCell] - points[6 * iCell + 3]);
                        float l = (float)Math.Sqrt(x * x + 2);
                        delta = new Vector3(x / l, 1 / l, 1 / l);
                    }
                    else if (points[6 * iCell] != points[6 * iCell + 3])
                    {
                        float y = ((points[6 * iCell + 3] - points[6 * iCell])
                            + (points[6 * iCell + 5] - points[6 * iCell + 2]))
                            / (points[6 * iCell + 1] - points[6 * iCell + 4]);
                        float l = (float)Math.Sqrt(y * y + 2);
                        delta = new Vector3(1 / l, y / l, 1 / l);
                    }
                    else
                    {
                        float z = ((points[6 * iCell + 4] - points[6 * iCell + 1])
                            + (points[6 * iCell + 3] - points[6 * iCell]))
                            / (points[6 * iCell + 2] - points[6 * iCell + 5]);
                        float l = (float)Math.Sqrt(z * z + 2);
                        delta = new Vector3(1 / l, 1 / l, z / l);
                    }

                    vertices[4 * iCell] = new Vector3(10 * (points[6 * iCell] - xMin) / (Max - Min) - 5,
                        5 - 10 * (points[6 * iCell + 1] - yMin) / (Max - Min),
                        10 * (points[6 * iCell + 2] - zMin) / (Max - Min) - 5);
                    vertices[4 * iCell + 1] = new Vector3(10 * (points[6 * iCell + 3] - xMin) / (Max - Min) - 5,
                        5 - 10 * (points[6 * iCell + 4] - yMin) / (Max - Min),
                        10 * (points[6 * iCell + 5] - zMin) / (Max - Min) - 5);
                    vertices[4 * iCell + 2] = vertices[4 * iCell + 1] + f * delta;
                    vertices[4 * iCell + 3] = vertices[4 * iCell] + f * delta;
                }
            }
            //60000节点的判断、分片
            int meshNum, maxVerNum = 60000;
            outputFiles[Bgi] = outputFileDir + "_bg" + Bgi + ".givb";
            long jf = 0;
            if (verNum <= maxVerNum)//不足60000节点，无需划分
            {
                meshNum = 1;
                sumSize += sizeI * 2 + sizeV * verNum + 3 * sizeI * triNum + sizeF;
                CreateFileWithSize(outputFiles[Bgi], sumSize);
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[Bgi]))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        accessor.Write<int>(jf, ref meshNum);
                        jf += sizeI;
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
                CreateFileWithSize(outputFiles[Bgi], sumSize);
                jf = 0;
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(outputFiles[Bgi]))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        accessor.Write<int>(jf, ref meshNum);
                        jf += sizeI;
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
    }
}
