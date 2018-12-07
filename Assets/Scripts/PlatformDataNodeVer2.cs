using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlatformDataNodeVer2 : MonoBehaviour {

    #region Platform Manager Custom Events
    public delegate void UpdatePlatformDataNodeUI(PlatformDataNodeVer2 data);
    public static event UpdatePlatformDataNodeUI OnUpdatePlatformDataNodeUI;
    #endregion

    public bool Simulate;
    public bool Selected;
    public bool Program;
    //public bool IsAtPosition;

    public Color NextColor;
    public int i;
    public int j;
    public int row = 1;
    public float NextPosition = 0.0f;
    //public float y = 0.0f;

    [Header("Selected Node UI Elements")]
    public Text txtSelectedNodeName;
    public Text txtSelectedNodePosition;
    public Image imgSelectedNodeColor;

    [Header("Selected Node Program Elements")]
    //public Text txtSelectedProgramNodeName;
    //public Text txtSelectedProgramNodePosition;
    public Slider sliderSelectedProgramNodeHeight;
    public Image imgSelectedProgramNodeColor;

    Canvas myCanvas;

    private void OnEnable()
    {
        //UIManagerWithEvents.OnSliderValueChanged += UIManagerWithEvents_OnSliderValueChanged;
        //UIManagerWithEvents.OnToggleProgram += UIManagerWithEvents_OnToggleProgram;

        UIManager.OnNodeProgramChanged += UIManager_OnNodeProgramChanged;
    }

    private void OnDisable()
    {
        //UIManagerWithEvents.OnSliderValueChanged -= UIManagerWithEvents_OnSliderValueChanged;
        //UIManagerWithEvents.OnToggleProgram -= UIManagerWithEvents_OnToggleProgram;

        UIManager.OnNodeProgramChanged -= UIManager_OnNodeProgramChanged;
    }

    private void UIManagerWithEvents_OnToggleProgram(Toggle t)
    {
        Program = !Program;
    }

    private void UIManager_OnNodeProgramChanged(Slider s)
    {
        if (Program)
        {
            if (Selected)
            {
                transform.position = new Vector3(transform.position.x, s.value, transform.position.z);
                NextPosition = s.value;

                transform.gameObject.GetComponentInChildren<Text>().text = string.Format("{0:0.00}", NextPosition);
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        myCanvas = transform.GetComponentInChildren<Canvas>();
        myCanvas.enabled = false;

        NextPosition = transform.position.y;
        ResetDataNode();
    }

    public void ResetDataNode()
    {
        if(!Program)
        {
            Simulate = true;
            Selected = false;

            NextColor = Color.white;
            NextPosition = 0.0f;
            //sliderSelectedProgramNodeHeight.value = transform.position.y;
        }
        else
        {
            Simulate = true;
            Selected = false;
        }
    }

    public void SelectNode()
    {
        NextColor = Color.red;
        //NextPosition = UnityEngine.Random.Range(0, 7);//UnityEngine.Random.Range(-Range, Range);
        Selected = true;
        Simulate = true;
        Program = true;

        myCanvas.enabled = true;

        //sliderSelectedProgramNodeHeight.value = transform.position.y;
        if (OnUpdatePlatformDataNodeUI != null)
            OnUpdatePlatformDataNodeUI(this);

        sliderSelectedProgramNodeHeight.value = transform.position.y;
    }

    void UpdateUI()
    {
        txtSelectedNodeName.text = transform.name;
        txtSelectedNodePosition.text = string.Format("{0:0.00}f", transform.position.y);

        //txtSelectedNodePosition.text = string.Format("Position: <{0:0.00},{1:0.00},{2:0.00}>",
        //    transform.position.x,
        //    transform.position.y,
        //    transform.position.z);

        //txtSelectedProgramNodePosition.text = string.Format("Position: <{0:0.00},{1:0.00},{2:0.00}>",
        //    transform.position.x,
        //    transform.position.y,
        //    transform.position.z);

        //imgSelectedNodeColor.color = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Program)
        {
            if (transform.GetComponentInChildren<Canvas>().isActiveAndEnabled)
                transform.GetComponentInChildren<Canvas>().enabled = true;
            
            if (Selected)
            {
                transform.gameObject.GetComponent<Renderer>().material.color = Color.blue;

                UpdateUI();
            }
            return;
        }

        if (!Program)
        {
            if (Simulate)
            {
                Debug.Log("SIM");
                //if (currentSelection != null)
                {
                    transform.position = Vector3.Lerp(transform.position,
                                                    new Vector3(transform.position.x,
                                                                NextPosition,
                                                                transform.position.z),
                                                    Time.deltaTime*5);

                    if (NearlyEquals(transform.position.y, NextPosition))
                    {
                        Simulate = false;
                        NextPosition = 0;
                    }

                    transform.gameObject.GetComponentInChildren<Text>().text = string.Format("{0:0.00}", transform.position.y);

                }
            }
            else
            {



            }
        }
    }

    public static bool NearlyEquals(float? value1, float? value2, float unimportantDifference = 0.01f)
    {
        if (value1 != value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return Math.Abs(value1.Value - value2.Value) < unimportantDifference;
        }

        return true;
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2}", i, j, NextPosition);
    }

    public void setNextPosition(PlatformConfigurationData pcd)
    {
        Simulate = true;
        int flup = (row + i) - pcd.M;

        if (flup < 0)
            flup = (row + i);
            //NextPosition = 0.0f;
        //else
            NextPosition = pcd.positions[flup,j];

        //Debug.Log(NextPosition);
        row -= 1;
        if (row < 1)
            row = pcd.M;

        //NextPosition = UnityEngine.Random.Range(0.0f, 5.0f);
    }

    public float getNextPosition(int r, PlatformConfigurationData pcd)
    {
        return 1.0f;
    }

    public float getCurrentPos()
    {
        return transform.position[1];
    }

}
