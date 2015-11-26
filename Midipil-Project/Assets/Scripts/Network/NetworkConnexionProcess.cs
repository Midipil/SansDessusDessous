using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkConnexionProcess : MonoBehaviour
{
	// Network variables
    public GameObject networkPrefab;
	private GameObject networkManager;
	private NetworkManager networkManagerScript;
	private HostData[] hostList = null;
    private bool isSetHostListDone = false;

    // World
    private bool testOculusView = false;
    public GameObject bearBot;
    public GameObject egg;

    // UI 
    public GameObject[] MenuPanels;
    private GameObject hostListModel;
    private Transform hostListContent;
    private bool isCameraMovementEnable = false;
    private bool isBackSelected = false;
    public Text listRoomText;

    // Information display
    public Text informationText;
	private bool displayMessage = false;
	private string message;

    public delegate void delegateFunction();
    private delegateFunction[] HomeFunctions;

    public const float AXIS_FREEZE_DELAY = 0.08f;
    public float lastAxisInputChecked;

    // Menu states variables
    enum HomeChoice
    {
        CREATE,
        JOIN,
        QUIT,
        NUMBER_OF_CHOICE
    };
    private HomeChoice currentHomeState = HomeChoice.CREATE;

    enum ConnexionState { 
        TwoPlayers, 
        RoomList, 
        WaitingRoom, 
        Play 
    };
	private ConnexionState currentMenu = ConnexionState.TwoPlayers;


    /// ==================================================================
    /// UNITY MONOBEHAVIOUR CALLS
    /// ================================================================== 
	void Awake () {
		// Retrieve or instantiate the NetworkManager gameobject which wear the "NetworkManager" tag
		if (GameObject.FindGameObjectWithTag ("NetworkManager") == null) {
			Debug.Log ("Instantiating NetworkManager");
			Instantiate(networkPrefab, networkPrefab.transform.position, networkPrefab.transform.rotation);
			networkManager = GameObject.FindGameObjectWithTag ("NetworkManager");
			networkManagerScript = networkManager.GetComponent<NetworkManager>();
		}
		else{
			Debug.Log ("Retrieving NetworkManager ");
			networkManager = GameObject.FindGameObjectWithTag ("NetworkManager");
			networkManagerScript = networkManager.GetComponent<NetworkManager>();
		}

		networkManagerScript.Intialize(this);
    }


	void Start () {

        // If the application detect an Oculus
        if (UnityEngine.VR.VRDevice.isPresent || testOculusView)
        {
            isCameraMovementEnable = false;
            bearBot.SetActive(false);
        }
        else {
            Camera.main.fieldOfView = 60;
            isCameraMovementEnable = true;
            egg.SetActive(false);
        }

        // Disable mouse curser
        // Cursor.visible = false;

        HomeFunctions = new delegateFunction[3]
        {
            OnCreateRoom,
            OnJoinRoom,
            OnQuit
        };
    
        DisplayCurrentMenu();
        SetCurrentStateToggleOn();
        lastAxisInputChecked = Time.time;
    }

    void Update()
    {
        // INPUT 
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Quit"))
        {
            Application.Quit();
        }
        MenuNavigation();

        // MOVEMENT : Slowly rotate the camera
        /*
        if (isCameraMovementEnable)
            Camera.main.transform.Rotate(new Vector3(0, 1, 0), 0.005f);
        */
    }


    /// ==================================================================
    /// NAVIGATION
    /// ================================================================== 
    public void MenuNavigation()
    {
        float now = Time.realtimeSinceStartup;
        bool checkAxis = false;
        if(now - lastAxisInputChecked >= AXIS_FREEZE_DELAY)
        {
            checkAxis = true;
            lastAxisInputChecked = now;
        }
        
        // TODO : add XBox controls
        if (currentMenu == ConnexionState.TwoPlayers)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || (checkAxis && (Input.GetAxisRaw("Menu_Vertical") > 0.0)))
            {
                UpdateSelectedState(-1);
                SetCurrentStateToggleOn();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || (checkAxis && (Input.GetAxisRaw("Menu_Vertical") < 0.0)))
            {
                UpdateSelectedState(1);
                SetCurrentStateToggleOn();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
            {
                ValidateHomeChoice();
            }
        }
        else if (currentMenu == ConnexionState.WaitingRoom || currentMenu == ConnexionState.RoomList)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || (checkAxis && Mathf.Abs(Input.GetAxisRaw("Menu_Vertical")) >= 0.1f)) {
                // Preselect qui button
                UnityEngine.UI.Toggle toggle = MenuPanels[(int)currentMenu].GetComponentInChildren<Toggle>();
                toggle.isOn = true;
                isBackSelected = true;
            }
            if (isBackSelected && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            {
                OnBack();
            }
        }
    }

    /// ==================================================================
    /// DISPLAY
    /// ================================================================== 
    private void DisplayCurrentMenu()
    {
        isBackSelected = false;

        for (int i = 0; i < MenuPanels.Length; ++i)
        {
            if (i ==  (int)currentMenu)
                MenuPanels[i].SetActive(true);
            else
                MenuPanels[i].SetActive(false);
        }

        if (currentMenu == ConnexionState.TwoPlayers)
        {
            isSetHostListDone = false;
            currentHomeState = HomeChoice.CREATE;
            SetCurrentStateToggleOn();
        }

        if (currentMenu == ConnexionState.WaitingRoom || currentMenu == ConnexionState.RoomList)
        {
            UnityEngine.UI.Toggle toggle = MenuPanels[(int)currentMenu].GetComponentInChildren<Toggle>();
            toggle.isOn = false;
        }
    }

    public void UpdateSelectedState(int value)
    {
        int newMode = (int)currentHomeState;
        newMode += (value);

        if (newMode >= (int)HomeChoice.NUMBER_OF_CHOICE) newMode = 0;
        else if (newMode < 0) newMode = (int)HomeChoice.NUMBER_OF_CHOICE - 1;

        currentHomeState = (HomeChoice)newMode;
    }

    public void SetCurrentStateToggleOn()
    {
        GameObject currentToggleGO = MenuPanels[(int)ConnexionState.TwoPlayers].transform.GetChild((int)currentHomeState).gameObject;
        UnityEngine.UI.Toggle currentToggle = currentToggleGO.GetComponent<UnityEngine.UI.Toggle>();
        currentToggle.isOn = true;
    }

    public void ValidateHomeChoice()
    {
        HomeFunctions[(int)currentHomeState]();
    }

    public void SetListRoomMessage(string text)
    {
        listRoomText.text = text;
    }

    /// ==================================================================
    /// DISPLAY
    /// ================================================================== 
    // HOME =================================
    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnCreateRoom()
    {
        Debug.Log("CREATE");
        networkManagerScript.StartServer();
    }

    public void OnJoinRoom()
    {
        currentMenu = ConnexionState.RoomList;
        DisplayCurrentMenu();
        SetListRoomMessage("Retrieving server list...");
        RequestRoomList();
    }

    // SERVER LIST ROOM =================================
    public void OnBack()
    {
        if (currentMenu == ConnexionState.WaitingRoom)
        {
            Debug.Log("CLOSE SERVER");
            networkManagerScript.CloseServer();
        }
        currentMenu = ConnexionState.TwoPlayers;
        DisplayCurrentMenu();
    }

    private void RequestRoomList()
    {
        Debug.Log("REQUEST LIST");
        // Refresh the hosts list
        MasterServer.ClearHostList();
        networkManagerScript.RefreshHostList();
    }

    public void displayRoomList()
    {
        // Display the list of available hosts if it isn't empty
        Debug.Log("DISPLAY LIST");
        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; i++)
            {
                Debug.Log(hostList[i].gameName);
                GameObject join = Instantiate(hostListModel);
                join.transform.parent = hostListContent;

                Button button = join.GetComponent<Button>();
                Text text = join.GetComponentInChildren<Text>();
                text.text = hostList[i].gameName;

                Debug.Log("SERVER : "+ i);
                button.onClick.AddListener(delegate () { OnJoinServer(i); });
            }
        }
    }

    public void OnJoinServer(int i)
    {
        //Debug.Log("SERVER : " + i--);
        //Debug.Log("SERVER : " + i--);
       // networkManagerScript.JoinServer(hostList[0]);
    }


    /// ==================================================================
    /// SETTERS
    /// ================================================================== 
    public void setHostList(HostData[] hosts){
        if (!isSetHostListDone && hosts != null)
        {
            hostList = hosts;
            isSetHostListDone = true;

            //displayRoomList();
            SetListRoomMessage("Connecting to server " + hostList[0].gameName);
            networkManagerScript.JoinServer(hostList[0]);
        }
	}

	public void setCurrentStateWait(){
        Debug.Log("WAIT");
        currentMenu = ConnexionState.WaitingRoom;
        DisplayCurrentMenu();
    }
	public void setCurrentStateHome(){
		currentMenu = ConnexionState.TwoPlayers;
        DisplayCurrentMenu();
    }

	public void stopDisplayMessage(){
        informationText.transform.parent.gameObject.SetActive(false);
    }
	public void setMessage(string msg){
        Debug.Log("HEY");
		message = msg;
		displayMessage = true;
        informationText.text = message;
        informationText.transform.parent.gameObject.SetActive(true);
        Invoke("stopDisplayMessage", 5f);
	}


    /// ==================================================================
    /// LOADING NEXT SCENE FUNCTIONS
    /// ==================================================================
    public void PlayAsServer(){
        Debug.LogError("PLAY AS SERVER !");

		currentMenu = ConnexionState.Play;

		// The networkManager won't be destroy
		DontDestroyOnLoad (networkManager);

		Application.LoadLevel("Main");
	}

    public void PlayAsClient()
    {
        Debug.LogError("PLAY AS CLIENT !");

        currentMenu = ConnexionState.Play;

        // The networkManager won't be destroy
        DontDestroyOnLoad(networkManager);

        Application.LoadLevel("Main");
    }
}
