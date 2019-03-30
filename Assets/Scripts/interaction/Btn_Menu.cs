using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Menu : MonoBehaviour {
    private Button btnMenu;//菜单按钮
    public static bool IsMenuOn = true;//判断当前菜单是否开启
    private GameObject panelMenu;//菜单面板
    private Sprite MenuOn, MenuOff;

    void Start()
    {
        btnMenu = GameObject.Find("Canvas/PanelGiv/PanelLeft/ButtonMenu").GetComponent<Button>();
        panelMenu = GameObject.Find("Canvas/PanelGiv/PanelLeft/PanelMenu");
        MenuOn = Resources.Load<Sprite>("Sprites/menu1");
        MenuOff = Resources.Load<Sprite>("Sprites/menu2");
        btnMenu.onClick.AddListener(OnClickMenuOn);
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
}
