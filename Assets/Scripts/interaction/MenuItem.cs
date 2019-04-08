using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour {
    //开关菜单
    private Button btnMenu;
    public bool IsMenuOn = true;
    private GameObject panelMenu;
    private Sprite MenuOn, MenuOff;
    //监控Area变化与否与具体值
    public bool isAreaChange = false;
    public int areaIndex = 0;
    private Dropdown dAera;
    //监控MeshOn变化与否与具体值
    public bool isMeshOnChange = false;
    public bool isMesh = true;
    private Toggle tMesh;
    //监控ScalarOn变化与否与具体值，获取目前的scalarIndex
    public bool isScalarOnChange = false;
    public bool isScalar = true;
    private Toggle tScalar;
    public bool isScalarChange = false;
    public int scalarIndex = 0;
    private Dropdown dScalar;
    //监控VectorOn变化与否与具体值，获取目前的vectorIndex
    public bool isVectorOnChange = false;
    public bool isVector = false;
    private Toggle tVector;
    public bool isVectorChange = false;
    public int vectorIndex = 0;
    private Dropdown dVector;
    //监控ScaleOn变化与否与具体值，获取目前的scaleIndex
    public bool isVerAdd = false;
    public bool isScaleOnChange = false;
    public bool isScale = false;
    private Toggle tScale;
    public bool isScaleChange = false;
    public int scaleIndex = 0;
    private Dropdown dScale;
    //监控time变化与否与具体值
    private bool isPlay = true;
    private Sprite spriteOn, spriteOff;
    private Button bPlay;
    private Slider sTime;
    public int timeNow = 0;
    public int timeSum;
    private Text textTime;

	// Use this for initialization
	void Start () {
        btnMenu = GameObject.Find("Canvas/PanelGiv/PanelLeft/ButtonMenu").GetComponent<Button>();
        panelMenu = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu");
        MenuOn = Resources.Load<Sprite>("Sprites/menu1");
        MenuOff = Resources.Load<Sprite>("Sprites/menu2");
        dAera = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/DropdownArea").GetComponent<Dropdown>();
        tMesh = (Toggle)GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleMesh").GetComponent<Toggle>();
        tScalar = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleSca").GetComponent<Toggle>();
        dScalar = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleSca/DropdownMesh").GetComponent<Dropdown>();
        tVector = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleVec").GetComponent<Toggle>();
        dVector = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleVec/DropdownMesh").GetComponent<Dropdown>();
        tScale = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleDeform").GetComponent<Toggle>();
        dScale = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleDeform/DropdownMesh").GetComponent<Dropdown>();
        bPlay = GameObject.Find("Canvas/PanelGiv/PanelBottom/ButtonPlay").GetComponent<Button>();
        spriteOn = Resources.Load<Sprite>("Sprites/btnOn");
        spriteOff = Resources.Load<Sprite>("Sprites/btnOff");
        sTime = GameObject.Find("Canvas/PanelGiv/PanelBottom/SliderTime").GetComponent<Slider>();
        textTime = GameObject.Find("Canvas/PanelGiv/PanelBottom/TextTime").GetComponent<Text>();
        btnMenu.onClick.AddListener(OnClickMenuOn);
        bPlay.onClick.AddListener(OnClickPlay);
        if (!isVerAdd)
            GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu/ToggleDeform").SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (areaIndex == dAera.value)
            isAreaChange = false;
        else
        {
            isAreaChange = true;
            areaIndex = dAera.value;
        }
        if (isMesh == tMesh.isOn)
            isMeshOnChange = false;
        else
        {
            isMeshOnChange = true;
            isMesh = tMesh.isOn;
        }
        if (isScalar == tScalar.isOn)
            isScalarOnChange = false;
        else
        {
            isScalarOnChange = true;
            isScalar = tScalar.isOn;
        }
        if (isScalar)
        {
            if (scalarIndex == dScalar.value)
                isScalarChange = false;
            else
            {
                isScalarChange = true;
                scalarIndex = dScalar.value;
            }
        }
        if (isVector == tVector.isOn)
            isVectorOnChange = false;
        else
        {
            isVectorOnChange = true;
            isVector = tVector.isOn;
        }
        if (isVector)
        {
            if (vectorIndex == dVector.value)
                isVectorChange = false;
            else
            {
                isVectorChange = true;
                vectorIndex = dVector.value;
            }
        }
        if (isScale == tScale.isOn)
            isScaleOnChange = false;
        else
        {
            isScaleOnChange = true;
            isScale = tScale.isOn;
        }
        if (isScale)
        {
            if (scaleIndex == dScale.value)
                isScaleChange = false;
            else
            {
                isScaleChange = true;
                scaleIndex = dScale.value;
            }
        }
        if (timeNow - (timeSum - 1) * sTime.value < 1 && timeNow - (timeSum - 1) * sTime.value > -1)
        {
            if (isPlay)
            {
                timeNow++;
                if (timeNow == timeSum)
                    timeNow = 0;
                sTime.value = timeNow * 1f / (timeSum - 1 + 1e-16f);
                textTime.text = "Time:" + timeNow;
            }
        }
        else
        {
            timeNow = (int)((timeSum - 1)* sTime.value);
            if (timeNow == timeSum)
                timeNow = 0;
            sTime.value = timeNow * 1f / (timeSum - 1 + 1e-16f);
            textTime.text = "Time:" + timeNow;
        }
    }

    public void OnClickMenuOn()
    {
        if (IsMenuOn)
        {
            panelMenu.SetActive(false);
            btnMenu.image.sprite = MenuOn;
            IsMenuOn = false;
        }
        else
        {
            panelMenu.SetActive(true);
            btnMenu.image.sprite = MenuOff;
            IsMenuOn = true;
        }
    }

    void OnClickPlay()
    {
        if (isPlay)
        {
            bPlay.GetComponent<Image>().sprite = spriteOff;
            isPlay = false;
        }
        else
        {
            bPlay.GetComponent<Image>().sprite = spriteOn;
            isPlay = true;
        }
    }



}
