using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class ChooseFileType : MonoBehaviour {
    private GameObject panelReadFiles;//启动界面
    private Button ButtonOK, ButtonCancel;//启动界面的“确定”按钮、“取消”按钮
    private Toggle Toggle1, Toggle2, Toggle3;//启动界面的giv复选框、vtk复选框、obj复选框
    public string[] pathOpen;
    public string pathSave;

    private GameObject panelVar;//变量选择界面
    private GameObject panelGiv;//可视化界面

    private GameObject newObject;

    // Use this for initialization
    void Start () {
        panelReadFiles = GameObject.Find("Canvas/PanelReadFiles");
        ButtonOK = (Button)GameObject.Find("Canvas/PanelReadFiles/ButtonOK").GetComponent<Button>();
        ButtonCancel = (Button)GameObject.Find("Canvas/PanelReadFiles/ButtonCancel").GetComponent<Button>();
        Toggle1 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle1").GetComponent<Toggle>();
        Toggle2 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle2").GetComponent<Toggle>();
        Toggle3 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle3").GetComponent<Toggle>();

        panelVar = GameObject.Find("Canvas/PanelVar");
        panelGiv = GameObject.Find("Canvas/PanelGiv");

        panelVar.SetActive(false);//变量选择界面隐藏
        panelGiv.SetActive(false);//可视化界面隐藏
        ButtonOK.onClick.AddListener(onClickOK);
        ButtonCancel.onClick.AddListener(onClickCancel);
    }

    void onClickOK()
    {
        if (Toggle1.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("读取.giv文件", Application.dataPath, "giv", true);
            if (pathOpen.Length > 0)
            {
                panelReadFiles.SetActive(false);//启动界面隐藏
                panelGiv.SetActive(true);//可视化界面显示
                newObject = (GameObject)Instantiate(Resources.Load("Prefabs/ReadGiv"));
                newObject.GetComponent<ReadGiv>().files = pathOpen;
                newObject.transform.parent = GetComponent<Transform>();
            }
        }
        else if (Toggle2.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("读取.vtk文件", Application.dataPath, "vtk", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("生成.giv文件", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    panelReadFiles.SetActive(false);//启动界面隐藏
                    panelVar.SetActive(true);//变量选择界面显示
                    panelGiv.SetActive(true);//可视化界面隐藏
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/CreateFem"));
                    newObject.GetComponent<CreateFem>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateFem>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
        else if (Toggle3.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("读取.obj文件", Application.dataPath, "obj", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("生成.giv文件", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    panelReadFiles.SetActive(false);//启动界面隐藏
                    panelGiv.SetActive(true);//可视化界面隐藏
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/CreateContour"));
                    newObject.GetComponent<CreateContour>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateContour>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
        else
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("读取.vtk文件", Application.dataPath, "vtk", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("生成.giv文件", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    panelReadFiles.SetActive(false);//启动界面隐藏
                    panelVar.SetActive(true);//变量选择界面显示
                    panelGiv.SetActive(true);//可视化界面隐藏
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/CreateFems"));
                    newObject.GetComponent<CreateFems>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateFems>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
    }
    void onClickCancel()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
