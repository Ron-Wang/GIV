using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Restore : MonoBehaviour {
    private Button btnRestore;//复原按钮
    private GameObject Objects;//父物体

    // Use this for initialization
    void Start()
    {
        btnRestore = GameObject.Find("Canvas/PanelGiv/PanelRight/ButtonRestore").GetComponent<Button>();
        Objects = GameObject.Find("Objects");
        btnRestore.onClick.AddListener(OnClickRestore);
    }

    public void OnClickRestore()
    {
        //父物体复原到原来的位置和角度
        Objects.transform.SetPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
    }
}
