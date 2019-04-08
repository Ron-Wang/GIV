using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem1 : MonoBehaviour {
    //开关菜单
    private Button btnMenu;
    public bool IsMenuOn = true;
    private GameObject panelMenu1;
    private Sprite MenuOn, MenuOff;
    //监控Color变化与否与具体值
    public bool isColorChange = false;
    public int colorIndex = 0;
    private Dropdown dColor;
    //监控MeshOn变化与否与具体值
    public bool isMeshOnChange = false;
    public bool isMesh = true;
    private Toggle tMesh;
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
        panelMenu1 = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu1");
        MenuOn = Resources.Load<Sprite>("Sprites/menu1");
        MenuOff = Resources.Load<Sprite>("Sprites/menu2");
        dColor = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu1/DropdownColor").GetComponent<Dropdown>();
        tMesh = (Toggle)GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu1/ToggleMesh").GetComponent<Toggle>();
        bPlay = GameObject.Find("Canvas/PanelGiv/PanelBottom/ButtonPlay").GetComponent<Button>();
        spriteOn = Resources.Load<Sprite>("Sprites/btnOn");
        spriteOff = Resources.Load<Sprite>("Sprites/btnOff");
        sTime = GameObject.Find("Canvas/PanelGiv/PanelBottom/SliderTime").GetComponent<Slider>();
        textTime = GameObject.Find("Canvas/PanelGiv/PanelBottom/TextTime").GetComponent<Text>();
        btnMenu.onClick.AddListener(OnClickMenuOn);
        bPlay.onClick.AddListener(OnClickPlay);
    }
	
	// Update is called once per frame
	void Update () {
        if (colorIndex == dColor.value)
            isColorChange = false;
        else
        {
            isColorChange = true;
            colorIndex = dColor.value;
        }
        if (isMesh == tMesh.isOn)
            isMeshOnChange = false;
        else
        {
            isMeshOnChange = true;
            isMesh = tMesh.isOn;
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
            timeNow = (int)((timeSum - 1) * sTime.value);
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
            panelMenu1.SetActive(false);
            btnMenu.image.sprite = MenuOn;
            IsMenuOn = false;
        }
        else
        {
            panelMenu1.SetActive(true);
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
