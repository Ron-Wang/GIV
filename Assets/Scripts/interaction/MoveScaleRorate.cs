using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoveScaleRorate : MonoBehaviour {
    private Button btnRorate;
    private bool IsRestore = true;
    public Vector3 mouse0, mouse1;
    private Vector3 btnTran0, btnTran1;
    private float speedMove, speedRorate, speedScale;
    public Vector2 st0, st1, od0, od1;
    private GameObject Objects;


    // Use this for initialization
    void Start()
    {
        btnRorate = GameObject.Find("Canvas/PanelGiv/PanelRight/ButtonRorate").GetComponent<Button>();
        btnTran0 = btnRorate.GetComponent<Transform>().position;
        Objects = GameObject.Find("Objects");
        speedMove = 0.01f;
        speedRorate = 0.05f;
        speedScale = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Btn_Rorate.IsRorate)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                    mouse1 = mouse0 = Input.mousePosition;
                mouse1 = Input.mousePosition;
                btnTran1 = btnRorate.GetComponent<Transform>().position + (mouse1 - mouse0);
                float distance = Vector3.Distance(btnTran0, btnTran1);
                if (distance < 50)
                    btnRorate.transform.Translate((mouse1 - mouse0));
                Objects.transform.Rotate(new Vector3((btnTran1 - btnTran0).y, -(btnTran1 - btnTran0).x) * speedRorate, Space.World);
                //Objects.transform.RotateAround(new Vector3(-5, 5, -5), new Vector3((btnTran1 - btnTran0).y, -(btnTran1 - btnTran0).x), speedRorate*100);
                mouse0 = mouse1;
            }
            if (IsRestore)
                IsRestore = false;
        }
        else
        {
            if (!IsRestore)
            {
                btnRorate.GetComponent<Transform>().position = btnTran0;
                IsRestore = true;
            }
            btnTran0 = btnRorate.GetComponent<Transform>().position;
        }


        if (Btn_Touch.IsTouch)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                    mouse1 = mouse0 = Input.mousePosition;
                mouse1 = Input.mousePosition;
                Objects.transform.Translate((mouse1 - mouse0).x * speedMove, (mouse1 - mouse0).y * speedMove, 0, Space.World);
                mouse0 = mouse1;
            }
            if (Input.touchCount >= 2)
            {
                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    st0 = od0 = Input.GetTouch(0).position;
                    st1 = od1 = Input.GetTouch(1).position;
                    return;
                }
                st0 = Input.GetTouch(0).position;
                st1 = Input.GetTouch(1).position;
                if (Vector2.Distance(st0, st1) != Vector2.Distance(od0, od1))
                {
                    Objects.transform.Translate(0, 0, -(Vector2.Distance(st0, st1) - Vector2.Distance(od0, od1)) * speedScale, Space.World);
                }
                od0 = st0;
                od1 = st1;
            }
        }
        Objects.transform.Translate(0, 0, -5 * Input.GetAxis("Mouse ScrollWheel"), Space.World);
    }
}
