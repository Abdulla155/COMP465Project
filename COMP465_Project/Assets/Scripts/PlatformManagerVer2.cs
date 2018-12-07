using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlatformManagerVer2 : PlatformGenericSinglton<PlatformManagerVer2>
{

    #region Platform Manager Custom Events
    public delegate void PlatformManagerChanged(PlatformConfigurationData data);
    public static event PlatformManagerChanged OnPlatformManagerChanged;

    public delegate void PlatformManagerUpdateUI();
    public static event PlatformManagerUpdateUI OnPlatformManagerUpdateUI;
    #endregion

    public enum ColorShade
    {
        GrayScale,
        RedScale,
        GreenScale,
        BlueScale,
        Random
    }

    public GameObject PlatformBasePref;
    public int oldM;
    public int oldN;
    public int row = 0;

    public PlatformConfigurationData configurationData = new PlatformConfigurationData();
    float spaceX = 0.0f;
    float spaceZ = 0.0f;

    public GameObject[,] platformNodes;

    public bool SimulateTest = false;
    public bool Program = false;

    //public ColorShade shade = ColorShade.GrayScale;

    #region Selected Node Information Display Variables
    [Header("Selected Node UI Controls")]
    GameObject currentSelection = null;
    //public Text txtSelectedNodeName;
    //public Text txtSelectedNodePosition;
    //public Image imgSelectedNodeColor;
    #endregion

    private void OnEnable()
    {
        UIManager.BuildPlatformOnClicked += UIManager_BuildPlatformOnClicked;
        UIManager.OnWriteProgramData += UIManager_OnWriteProgramData;
        //UIManager.OnUpdatePlatformNode += UIManager_OnUpdatePlatformNode;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        UIManager.BuildPlatformOnClicked -= UIManager_BuildPlatformOnClicked;
        UIManager.OnWriteProgramData -= UIManager_OnWriteProgramData;
        //UIManager.OnUpdatePlatformNode -= UIManager_OnUpdatePlatformNode;

        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void UIManager_BuildPlatformOnClicked(PlatformConfigurationData pcd)
    {
        configurationData = pcd;

        BuildPlatform();
        //oldM = pcd.M;
        //oldN = pcd.N;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        DestroyPlatform();

        if (SceneManager.GetActiveScene().name.Contains("Program"))
        {
            configurationData = ReadConfigData();
            Program = true;
        }
        else
            Program = false;
        
        if (SceneManager.GetActiveScene().name.Contains("Simulate"))
        {
            SimulateTest = true;
            configurationData = ReadConfigData();
        }
        else
            SimulateTest = false;
        
        if (!SceneManager.GetActiveScene().name.Contains("MainMenu"))
            BuildPlatform();

        if (platformNodes != null)
            if (OnPlatformManagerUpdateUI != null)
                OnPlatformManagerUpdateUI();

        Debug.Log(SceneManager.GetActiveScene().name);
    }

    //private void UIManager_OnUpdatePlatformNode(PlatformDataNodeVer2 data)
    //{
    //    PlatformDataNodeVer2 pdn = platformNode[data.i, data.j].GetComponent<PlatformDataNodeVer2>();
    //    pdn = data;
    //}


    private void UIManager_OnWriteProgramData()
    {
        // we will save the platform configuration data 
        // we will save the platform node program data
        Debug.Log("SAVING PLATFORM PROGRAM DATA ... SIMULATION");
        //Debug.Log(configurationData.ToString());

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Application.dataPath, "WriteLines.txt")))
        {
            outputFile.WriteLine(configurationData.ToString());
            for (int i = 0; i < configurationData.M; i++)
            {
                for (int j = 0; j < configurationData.N; j++)
                {
                    //Debug.Log(platformNode[i, j].GetComponent<PlatformDataNodeVer2>().ToString());
                    outputFile.WriteLine(platformNodes[i, j].GetComponent<PlatformDataNodeVer2>().ToString());
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    #region BUILD PLATFORM FROM UI

    public void DestroyPlatform()
    {
        // check to see if there is no platform currently configured
        // if there is one, delete it
        Debug.Log("Platform Destroyed");
        if (platformNodes != null)
        {
            for (int i = 0; i < oldM; i++)
            {
                for (int j = 0; j < oldN; j++)
                {
                    Destroy(platformNodes[i, j], 0.1f);
                }
            }

            platformNodes = null;
        }
    }

    public void BuildPlatform()
    {
        Debug.Log("Building");
        DestroyPlatform();

        platformNodes = new GameObject[configurationData.M, configurationData.N];

        spaceX = 0;
        spaceZ = 0;

        for (int i = 0; i < configurationData.M; i++)
        {
            spaceZ = 0.0f;
            for (int j = 0; j < configurationData.N; j++)
            {
                float x = (i * 1) + spaceX;
                float z = (j * 1) + spaceZ;
                
                var platformBase = Instantiate(PlatformBasePref,
                                                new Vector3(x, 0, z),
                                                Quaternion.identity);

                platformBase.name = string.Format("Node[{0},{1}]", i, j);

                platformBase.AddComponent<PlatformDataNodeVer2>();

                platformNodes[i, j] = platformBase;

                PlatformDataNodeVer2 pdn = platformBase.transform.GetComponent<PlatformDataNodeVer2>();
                pdn.Program = Program;
                pdn.i = i;
                pdn.j = j;

                spaceZ += configurationData.deltaSpace;
            }
            spaceX += configurationData.deltaSpace;
        }

        oldM = configurationData.M;
        oldN = configurationData.N;

        if (OnPlatformManagerChanged != null)
            OnPlatformManagerChanged(configurationData);
    }

    public void StartSimulationButtonClick()
    {
        SimulateTest = !SimulateTest;
    }
    #endregion

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

    // Update is called once per frame
    void Update()
    {

        // check to see if platform has been build
        if (platformNodes == null)
            return;

        //if (Input.GetKeyUp(KeyCode.T))
        //    SimulateTest = !SimulateTest;
        //if (Input.GetKeyUp(KeyCode.H))
        //    shade = ColorShade.GrayScale;
        //if (Input.GetKeyUp(KeyCode.R))
        //    shade = ColorShade.RedScale;
        //if (Input.GetKeyUp(KeyCode.G))
        //    shade = ColorShade.GreenScale;
        //if (Input.GetKeyUp(KeyCode.B))
        //    shade = ColorShade.BlueScale;
        //if (Input.GetKeyUp(KeyCode.E))
        //    shade = ColorShade.Random;
        //if (Input.GetKeyUp(KeyCode.W))
        //{
        //    RandomHeight += 1;
        //    txtPlatformYAxisRange.text = string.Format("{0}", RandomHeight);
        //}
        //if (Input.GetKeyUp(KeyCode.S))
        //{
        //    RandomHeight -= 1;
        //    if (RandomHeight < 0)
        //        RandomHeight = 0;
        //    txtPlatformYAxisRange.text = string.Format("{0}", RandomHeight);
        //}
        if (Input.GetKey(KeyCode.Q))
            Application.Quit();

        #region Object Selection
        // you can select only if in program mode/scene
        if (Program)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (IsPointerOverUIObject())
                    return;

                #region Screen To World
                RaycastHit hitInfo = new RaycastHit();  
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    #region COLOR

                    if (currentSelection != null)
                    {
                        PlatformDataNodeVer2 pdn = currentSelection.transform.GetComponent<PlatformDataNodeVer2>();
                        pdn.ResetDataNode();
                    }

                    currentSelection = hitInfo.transform.gameObject;
                    PlatformDataNodeVer2 newPdn = currentSelection.transform.GetComponent<PlatformDataNodeVer2>();
                    newPdn.SelectNode();

                    #endregion
                }
                else
                {
                    Debug.Log("No hit");
                }
                #endregion
            }
        }
        #endregion
        
        if(SimulateTest)
        {
            if(PlatformNodesDone())
            //for (int i = 0; i < configurationData.N; i += 1)
            {
                foreach (GameObject currentNode in platformNodes)//platformNodesGetRow(i))
                {
                    if (!currentNode.GetComponent<PlatformDataNodeVer2>().Simulate)
                    {
                        currentNode.GetComponent<PlatformDataNodeVer2>().setNextPosition(configurationData);
                        
                    }
                    
                }
            }
        }

    }

    public bool PlatformNodesDone()
    {
        foreach (GameObject currNode in platformNodes)
        {
            if (currNode.GetComponent<PlatformDataNodeVer2>().Simulate)
                return false;
        }
        return true;
        
    }

    public GameObject[] platformNodesGetRow(int i)
    {
        int a = 0;
        GameObject[] result = new GameObject[configurationData.N];

        foreach (GameObject currNode in platformNodes)
            if (currNode.GetComponent<PlatformDataNodeVer2>().i == i)
            {
                result[a] = currNode;
                a += 1;
            }
        return result;
    }

    /// <summary>
    /// Used to determine if we are over UI element or not.
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //foreach (var result in results)
        //{
        //    Debug.Log(result.gameObject.name);
        //}
        return results.Count > 0;
    }

    public PlatformConfigurationData ReadConfigData()
    {
        string line;
        string[] tokens;
        float[,] positions = new float[oldM, oldN]; 
        int i = 0;
        PlatformConfigurationData pcd = new PlatformConfigurationData();


        using (System.IO.StreamReader inputFile = new System.IO.StreamReader("Assets/WriteLines.txt"))
        {

            while ((line = inputFile.ReadLine()) != null)
            {
                tokens = line.Split(',');
                if (i == 0)
                {
                    pcd.M = int.Parse(tokens[0]);
                    pcd.N = int.Parse(tokens[1]);
                    pcd.deltaSpace = float.Parse(tokens[2]);
                    pcd.RandomHeight = float.Parse(tokens[3]);
                    positions = new float[pcd.M, pcd.N];
                }
                else
                {
                    positions[int.Parse(tokens[0]), int.Parse(tokens[1])] = float.Parse(tokens[2]);
                }
                i += 1;
            }
        }

        pcd.positions = positions;
        return pcd;

        

        /*

        int i = 0;
        while ((line = inputFile.ReadLine()) != null)
        {
            file[i] = line;
            print(line + " " + file[i]);
            i += 1;
        }
        string[] tokens = file[0].Split(',');

        pcd.M = int.Parse(tokens[0]);
        pcd.N = int.Parse(tokens[1]);
        pcd.deltaSpace = float.Parse(tokens[2]);
        pcd.RandomHeight = float.Parse(tokens[3]);

        return pcd;
        */
    }
}

