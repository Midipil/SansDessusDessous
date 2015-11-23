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
    public GameObject hostListModel;
    public Transform hostListContent;
    private bool isCameraMovementEnable = false;

    // Information display
    public Text informationText;
	private bool displayMessage = false;
	private string message;
		
	// Menu states variables
    enum HomeChoice
    {
        CREATE,
        JOIN,
        QUIT,
        NUMBER_OF_CHOICE
    };
    private HomeChoice currentHomeState = HomeChoice.CREATE;

	enum MenuState { 
        TwoPlayers, 
        RoomList, 
        WaitingRoom, 
        Play 
    };
	private MenuState currentMenu = MenuState.TwoPlayers;


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

		networkManagerScript.FindMenu();
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
        Cursor.visible = false;

        informationText.gameObject.SetActive(false);
        DisplayCurrentMenu();
	}

    void Update()
    {
        // INPUT 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        MenuNavigation();

        // MOVEMENT : Slowly rotate the camera
        /*
        if (isCameraMovementEnable)
        {
            Camera.main.transform.Rotate(new Vector3(0, 1, 0), 0.005f);
        }
        */
    }

    public void MenuNavigation()
    {
        // TODO : add XBox controls
        if (currentMenu == MenuState.TwoPlayers)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                UpdateSelectedState(1);
                SetCurrentStateToggleOn();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                UpdateSelectedState(-1);
                SetCurrentStateToggleOn();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return))
            {
                ValidateHomeChoice();
            }
        }
    }

    /// ==================================================================
    /// DISPLAY
    /// ================================================================== 
    private void DisplayCurrentMenu()
    {
        for (int i = 0; i < MenuPanels.Length; ++i)
        {
            if (i ==  (int)currentMenu)
                MenuPanels[i].SetActive(true);
            else
                MenuPanels[i].SetActive(false);
        }

        if (currentMenu == MenuState.TwoPlayers)
            isSetHostListDone = false;
    }

    public void UpdateSelectedState(int value)
    {
        Debug.Log((int)currentHomeState);
        int newMode = (int)currentHomeState;
        newMode += (value);

        if (newMode >= (int)HomeChoice.NUMBER_OF_CHOICE) newMode = 0;
        else if (newMode < 0) newMode = (int)HomeChoice.NUMBER_OF_CHOICE - 1;

        currentHomeState = (HomeChoice)newMode;
    }

    public void SetCurrentStateToggleOn()
    {
        GameObject currentToggleGO = MenuPanels[(int)MenuState.TwoPlayers].transform.GetChild((int)currentHomeState).gameObject;
        UnityEngine.UI.Toggle currentToggle = currentToggleGO.GetComponent<UnityEngine.UI.Toggle>();
        currentToggle.isOn = true;
    }

    public void ValidateHomeChoice()
    {

    }

    /*******************************************************
    * EVENTS
    ******************************************************/
    public void OnCreateRoom()
    {
        networkManagerScript.StartServer();
    }

    public void OnJoinRoom()
    {
        currentMenu = MenuState.RoomList;
        DisplayCurrentMenu();
        RequestRoomList();
    }

    public void OnBack()
    {
        currentMenu = MenuState.TwoPlayers;
        DisplayCurrentMenu();
        networkManagerScript.CloseServer();
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
            Debug.Log("HEYYYYYYYYYYYYYYY2");
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
        networkManagerScript.JoinServer(hostList[0]);
    }


    /*******************************************************
	 * Setter functions
	 ******************************************************/
    public void setHostList(HostData[] hosts){
        if (!isSetHostListDone && hosts != null)
        {
            hostList = hosts;

            isSetHostListDone = true;
            displayRoomList();
        }
	}

	public void setCurrentStateWait(){
		currentMenu = MenuState.WaitingRoom;
        DisplayCurrentMenu();
    }
	public void setCurrentStateNetwork(){
		currentMenu = MenuState.TwoPlayers;
        DisplayCurrentMenu();
    }

	public void stopDisplayMessage(){
        informationText.gameObject.SetActive(false);
    }
	public void setMessage(string msg){
		message = msg;
		displayMessage = true;
        informationText.text = message;
        informationText.gameObject.SetActive(true);
        Invoke("stopDisplayMessage", 2);
	}

	/*******************************************************
	 * Loading next scene functions
	 ******************************************************/
	public void PlayAsServer(){
        Debug.LogError("PLAY AS SERVER !");

		currentMenu = MenuState.Play;

		// The networkManager won't be destroy
		DontDestroyOnLoad (networkManager);

		Application.LoadLevel("Main");
	}

    public void PlayAsClient()
    {
        Debug.LogError("PLAY AS CLIENT !");

        currentMenu = MenuState.Play;

        // The networkManager won't be destroy
        DontDestroyOnLoad(networkManager);

        Application.LoadLevel("Main");
    }
}
