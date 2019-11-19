using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

public class Btn_Windows : MonoBehaviour {
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    private Button btnMin,btnMax,btnCancel;//窗口按钮：最小化、最大化、关闭
    private bool IsMax = false;//判断目前窗口状态

    // Use this for initialization
    void Start () {
        btnMin = GameObject.Find("PanelTitile/ButtonMin").GetComponent<Button>();
        btnMax = GameObject.Find("PanelTitile/ButtonMax").GetComponent<Button>();
        btnCancel = GameObject.Find("PanelTitile/ButtonCancel").GetComponent<Button>();
        btnMin.onClick.AddListener(OnClickMin);
        btnMax.onClick.AddListener(OnClickMax);
        btnCancel.onClick.AddListener(OnClickCancel);
    }
    //最小化窗口
    public void OnClickMin()
    {
        ShowWindow(GetForegroundWindow(), 2);
    }
    //最大化（或复原）窗口
    public void OnClickMax()
    {
        if (IsMax)
        {
            ShowWindow(GetForegroundWindow(), 1);
            IsMax = false;
        }
        else
        {
            ShowWindow(GetForegroundWindow(), 3);
            IsMax = true;
        }
    }
    //关闭窗口
    public void OnClickCancel()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
