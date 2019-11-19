using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.Threading.Tasks;

public class ChooseFileType : MonoBehaviour {
    private GameObject panelReadFiles;//启动界面
    private Button ButtonOK, ButtonCancel;//启动界面的“确定”按钮、“取消”按钮
    private Toggle Toggle1, Toggle2, Toggle3,Toggle4;//启动界面的giv复选框、vtk复选框、obj复选框
    public string[] pathOpen;
    public string pathSave;

    private GameObject panelVar;//变量选择界面
    private GameObject panelBg;//背景选择界面
    private GameObject panelGiv;//可视化界面

    private GameObject newObject;
    private bool isNext = false;

    // Use this for initialization
    void Start () {
        panelReadFiles = GameObject.Find("Canvas/PanelReadFiles");
        ButtonOK = (Button)GameObject.Find("Canvas/PanelReadFiles/ButtonOK").GetComponent<Button>();
        ButtonCancel = (Button)GameObject.Find("Canvas/PanelReadFiles/ButtonCancel").GetComponent<Button>();
        Toggle1 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle1").GetComponent<Toggle>();
        Toggle2 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle2").GetComponent<Toggle>();
        Toggle3 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle3").GetComponent<Toggle>();
        Toggle4 = (Toggle)GameObject.Find("Canvas/PanelReadFiles/Toggle3").GetComponent<Toggle>();

        panelVar = GameObject.Find("Canvas/PanelVar");
        panelBg = GameObject.Find("Canvas/PanelBackground");
        panelGiv = GameObject.Find("Canvas/PanelGiv");
        panelVar.SetActive(false);//变量选择界面隐藏
        panelBg.SetActive(false);//背景选择界面隐藏
        panelGiv.SetActive(false);//可视化界面隐藏
        ButtonOK.onClick.AddListener(onClickOK);
        ButtonCancel.onClick.AddListener(onClickCancel);
    }

    void onClickOK()
    {
        if (Toggle1.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("Read .giv Files", Application.dataPath, "giv", true);
            if (pathOpen.Length > 0)
            {
                isNext = true;
                newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/ReadGiv"));
                newObject.GetComponent<ReadGiv>().files = pathOpen;
                newObject.transform.parent = GetComponent<Transform>();
            }
        }
        else if (Toggle2.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("Read .vtk Files", Application.dataPath, "vtk", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("Create .giv Files", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    isNext = true;
                    panelVar.SetActive(true);//变量选择界面显示
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/CreateFem"));
                    newObject.GetComponent<CreateFem>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateFem>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
        else if (Toggle3.isOn)
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("Read .obj Files", Application.dataPath, "obj", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("Create .giv Files", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    isNext = true;
                    panelBg.SetActive(true);//背景界面显示
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/CreateContour"));
                    newObject.GetComponent<CreateContour>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateContour>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
        else
        {
            pathOpen = StandaloneFileBrowser.OpenFilePanel("Read .vtk Files", Application.dataPath, "vtk", true);
            if (pathOpen.Length > 0)
            {
                pathSave = StandaloneFileBrowser.SaveFilePanel("Create .giv Files", Application.dataPath, "Time_", "giv");
                if (pathSave != "")
                {
                    isNext = true;
                    panelVar.SetActive(true);//变量选择界面显示
                    newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Scripts/CreateFems"));
                    newObject.GetComponent<CreateFems>().inputFiles = pathOpen;
                    newObject.GetComponent<CreateFems>().outputFile = pathSave;
                    newObject.transform.parent = GetComponent<Transform>();
                }
            }
        }
        if (isNext)
        {
            panelReadFiles.SetActive(false);//启动界面隐藏
            panelGiv.SetActive(true);//可视化界面显示
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
