using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GK
{
    public class Section
    {
        public int nodeNum = 0;
        private int[] node0;
        private int[] node1;
        private float[] fraction;
        public Vector3[] vertices = null;
        public int[] triangles = null;
        private int direction;
        private float sectionValue;
        public Vector3 vMax, vMin;
        private float Max, Min;

        public Section(float[] points, int[] tri, int xyz, float value, Vector3 maxVer, Vector3 minVer)
        {
            //xyz截面方向，value截面值
            direction = xyz;
            sectionValue = value;
            vMax = maxVer;
            vMin = minVer;
            if (vMax.x - vMin.x > vMax.y - vMin.y)
            {
                if (vMax.x - vMin.x > vMax.z - vMin.z)
                {
                    Max = vMax.x;
                    Min = vMin.x;
                }
                else
                {
                    Max = vMax.z;
                    Min = vMin.z;
                }
            }
            else
            {
                if (vMax.y - vMin.y > vMax.z - vMin.z)
                {
                    Max = vMax.y;
                    Min = vMin.y;
                }
                else
                {
                    Max = vMax.z;
                    Min = vMin.z;
                }
            }


            int pointNum = points.Length / 3;
            int triNum = tri.Length / 3;
            //int[] mark = new int[pointNum * pointNum];
            List<long> mark = new List<long>();
            List<int> n0 = new List<int>();
            List<int> n1 = new List<int>();
            List<float> frac = new List<float>();
            List<Vector2> v2 = new List<Vector2>();
            List<Vector3> v3 = new List<Vector3>();

            int[] pointIndex = new int[] { 0, 1, 1, 2, 2, 0 };
            int ni0 = 0, ni1 = 0;
            float f = 0;
            for (int i = 0; i < triNum; ++i)
            {
                for (int pointIndexI = 0; pointIndexI < 3; ++pointIndexI)
                {
                    int triPoint0 = tri[3 * i + pointIndex[2 * pointIndexI]];
                    int triPoint1 = tri[3 * i + pointIndex[2 * pointIndexI + 1]];
                    bool isSection = false;
                    if (points[3 * triPoint0 + xyz] < value)
                    {
                        if (points[3 * triPoint1 + xyz] > value)
                        {
                            ni0 = triPoint0;
                            ni1 = triPoint1;
                            f = (value - points[3 * ni0 + xyz]) / (points[3 * ni1 + xyz] - points[3 * ni0 + xyz]);
                            isSection = true;
                        }
                        else if (points[3 * triPoint1 + xyz] == value)
                        {
                            ni0 = triPoint1;
                            ni1 = triPoint1;
                            f = 0;
                            isSection = true;
                        }
                    }
                    else if (points[3 * triPoint0 + xyz] > value)
                    {
                        if (points[3 * triPoint1 + xyz] < value)
                        {
                            ni0 = triPoint1;
                            ni1 = triPoint0;
                            f = (value - points[3 * ni0 + xyz]) / (points[3 * ni1 + xyz] - points[3 * ni0 + xyz]);
                            isSection = true;
                        }
                        else if (points[3 * triPoint1 + xyz] == value)
                        {
                            ni0 = triPoint1;
                            ni1 = triPoint1;
                            f = 0;
                            isSection = true;
                        }
                    }
                    else if (points[3 * triPoint0 + xyz] == value)
                    {
                        if (points[3 * triPoint1 + xyz] < value)
                        {
                            ni0 = triPoint0;
                            ni1 = triPoint0;
                            f = 0;
                            isSection = true;
                        }
                        else if (points[3 * triPoint1 + xyz] > value)
                        {
                            ni0 = triPoint0;
                            ni1 = triPoint0;
                            f = 0;
                            isSection = true;
                        }
                    }
                    if (isSection && !mark.Contains(ni0*pointNum + ni1)) 
                    {
                        n0.Add(ni0);
                        n1.Add(ni1);
                        frac.Add(f);
                        float xx, yy;
                        switch (xyz)
                        {
                            case 0:
                                xx = points[3 * ni0 + 1] + f * (points[3 * ni1 + 1] - points[3 * ni0 + 1]);
                                yy = points[3 * ni0 + 2] + f * (points[3 * ni1 + 2] - points[3 * ni0 + 2]);
                                v2.Add(new Vector2(xx, yy));
                                v3.Add(new Vector3(10 * (value - vMin.x) / (Max - Min) - 5
                                    , 5 - 10 * (xx - vMin.y) / (Max - Min), 10 * (yy - vMin.z) / (Max - Min) - 5));
                                break;
                            case 1:
                                xx = points[3 * ni0] + f * (points[3 * ni1] - points[3 * ni0]);
                                yy = points[3 * ni0 + 2] + f * (points[3 * ni1 + 2] - points[3 * ni0 + 2]);
                                v2.Add(new Vector2(xx, yy));
                                v3.Add(new Vector3(10 * (xx - vMin.x) / (Max - Min) - 5
                                    , 5 - 10 * (value - vMin.y) / (Max - Min), 10 * (yy - vMin.z) / (Max - Min) - 5));
                                break;
                            case 2:
                                xx = points[3 * ni0] + f * (points[3 * ni1] - points[3 * ni0]);
                                yy = points[3 * ni0 + 1] + f * (points[3 * ni1 + 1] - points[3 * ni0 + 1]);
                                v2.Add(new Vector2(xx, yy));
                                v3.Add(new Vector3(10 * (xx - vMin.x) / (Max - Min) - 5
                                    , 5 - 10 * (yy - vMin.y) / (Max - Min), 10 * (value - vMin.z) / (Max - Min) - 5));
                                break;
                        }
                        mark.Add(ni0 * pointNum + ni1);
                    }

                }
            }
            node0 = n0.ToArray();
            node1 = n1.ToArray();
            nodeNum = node0.Length;
            fraction = frac.ToArray();
            vertices = v3.ToArray();
            // Delaunay 三角剖分
            DelaunayTriangulation delaunayTriangulation = null;
            DelaunayCalculator delaunayCalculator = new DelaunayCalculator();
            if(vertices.Length > 0)
            {
                delaunayCalculator.CalculateTriangulation(v2, ref delaunayTriangulation);
                triangles = delaunayTriangulation.Triangles.ToArray();
            }
        }

        public void Scalars(float[] s, ref Color[] scalars,ref float max,ref float min)
        {
            scalars = new Color[nodeNum];
            float[] s_ = new float[nodeNum];
            max = float.MinValue;
            min = float.MaxValue;
            for (int i = 0; i < nodeNum; ++i)
            {
                s_[i] = s[node0[i]] + fraction[i] * (s[node1[i]] - s[node0[i]]);
                max = Math.Max(s_[i], max);
                min = Math.Min(s_[i], min);
            }
            if (max == min)
                for (int i = 0; i < nodeNum; ++i)
                {
                    scalars[i] = Color.blue;
                }
            else
                for (int i = 0; i < nodeNum; ++i)
                {
                    scalars[i] = FourColor((s_[i] - min) / (max - min));
                }
        }
        public void ReScalars(float[] s, ref Color[] scalars, float max, float min)
        {
            scalars = new Color[nodeNum];
            if (max == min)
                for (int i = 0; i < nodeNum; ++i)
                {
                    scalars[i] = Color.blue;
                }
            else
                for (int i = 0; i < nodeNum; ++i)
                {
                    scalars[i] = FourColor((s[node0[i]] + fraction[i] * (s[node1[i]] - s[node0[i]]) - min) / (max - min));
                }
        }


        public void VectorsQ(float[] v, ref Quaternion[] vectorsQ, ref float[] vectorsF,ref float max,ref float min)
        {
            vectorsQ = new Quaternion[nodeNum];
            vectorsF = new float[nodeNum];
            float[] v_ = new float[nodeNum];
            max = float.MinValue;
            min = float.MaxValue;
            Quaternion q = Quaternion.identity;
            Vector3 v0 = new Vector3(0, 0, 1);
            for (int i = 0; i < nodeNum; ++i)
            {
                float a0 = v[3 * node0[i]] + fraction[i] * (v[3 * node1[i]] - v[3 * node0[i]]),
                     a1 = v[3 * node0[i] + 1] + fraction[i] * (v[3 * node1[i] + 1] - v[3 * node0[i] + 1]),
                      a2 = v[3 * node0[i] + 2] + fraction[i] * (v[3 * node1[i] + 2] - v[3 * node0[i] + 2]);
                q.SetFromToRotation(v0, new Vector3(a0, a1, a2));
                vectorsQ[i] = q;
                v_[i] = Mathf.Sqrt(a0 * a0 + a1 * a1 + a2 * a2);
                max = Math.Max(v_[i], max);
                min = Math.Min(v_[i], min);
            }
            if (max == min)
                for (int i = 0; i < nodeNum; ++i)
                {
                    vectorsF[i] = 0.3f;
                }
            else
                for (int i = 0; i < nodeNum; ++i)
                {
                    vectorsF[i] = (v_[i] - min) / (max - min);
                }
        }
        public void ReVectorsQ(float[] v, ref float[] vectorsF, float max, float min)
        {
            if (max == min)
                for (int i = 0; i < nodeNum; ++i)
                {
                    vectorsF[i] = 0.3f;
                }
            else
                for (int i = 0; i < nodeNum; ++i)
                {
                    float a0 = v[3 * node0[i]] + fraction[i] * (v[3 * node1[i]] - v[3 * node0[i]]),
                     a1 = v[3 * node0[i] + 1] + fraction[i] * (v[3 * node1[i] + 1] - v[3 * node0[i] + 1]),
                      a2 = v[3 * node0[i] + 2] + fraction[i] * (v[3 * node1[i] + 2] - v[3 * node0[i] + 2]);

                    vectorsF[i] = (Mathf.Sqrt(a0 * a0 + a1 * a1 + a2 * a2) - min) / (max - min);
                }
        }

        public void Deformation(float[] def, ref Vector3[] deformation)
        {
            deformation = new Vector3[nodeNum];
            for (int i = 0; i < nodeNum; ++i)
            {
                float a0 = def[3 * node0[i]] + fraction[i] * (def[3 * node1[i]] - def[3 * node0[i]]),
                     a1 = def[3 * node0[i] + 1] + fraction[i] * (def[3 * node1[i] + 1] - def[3 * node0[i] + 1]),
                      a2 = def[3 * node0[i] + 2] + fraction[i] * (def[3 * node1[i] + 2] - def[3 * node0[i] + 2]);
                deformation[i] = new Vector3((a0 - Min) / (Max - Min), (a1 - Min) / (Max - Min), (a2 - Min) / (Max - Min));
            }
        }
        private Color FourColor(float value)
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

    }
}