using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {

	// Public variables
	public GameObject networkPrefab;
	public GameObject mainCamera;
    public GameObject[] panels;

	// Network variables
	private GameObject networkManager;
	private NetworkManager networkManagerScript;
	private HostData[] hostList = null;
    private bool isDone = false;

    public GameObject model;
    public Transform content;

    public Text m_Msg;
    
	// Gui variables
	private bool displayMessage = false;
	private string message;
		
	// Menu states variables
	enum MenuState { TwoPlayers, RoomList, WaitingRoom , Play };
	private MenuState currentMenu = MenuState.TwoPlayers;
	

	/*******************************************************
	 * Initialisation functions
	 ******************************************************/
	
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

        // Enable mouse curser
        //Screen.showCursor = true;

        m_Msg.gameObject.SetActive(false);
        DisplayCurrentMenu();
	}

	void Update(){

        if (Input.GetKeyDown(KeyCode.C)){
            OnCreateRoom();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            OnJoinRoom();
        }
    }


	/*******************************************************
	 * DISPLAY
	 ******************************************************/
	
        /*
	void OnGUI()
	{
		// Menus
		if(currentMenu == MenuState.TwoPlayers)
			displayTwoPlayersMenu();
		else if (currentMenu == MenuState.WaitingRoom)
			displayWaitingRoom();
		else if (currentMenu == MenuState.RoomList)
			displayRoomList();

		if (displayMessage) {
			GUI.Label (new Rect (0, 0,500,100), message);
		}
	}
    */


    private void DisplayCurrentMenu()
    {
        for (int i = 0; i < panels.Length; ++i)
        {
            if (i ==  (int)currentMenu)
                panels[i].SetActive(true);
            else
                panels[i].SetActive(false);
        }

        if (currentMenu == MenuState.TwoPlayers)
            isDone = false;
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
                GameObject join = Instantiate(model);
                join.transform.parent = content;

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
        if (!isDone && hosts != null)
        {
            hostList = hosts;
        
            isDone = true;
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
        m_Msg.gameObject.SetActive(false);
    }
	public void setMessage(string msg){
		message = msg;
		displayMessage = true;
        m_Msg.text = message;
        m_Msg.gameObject.SetActive(true);
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
