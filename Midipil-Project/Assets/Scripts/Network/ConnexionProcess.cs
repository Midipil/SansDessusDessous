using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnexionProcess : MonoBehaviour {

	// Public variables
	public GameObject networkPrefab;
	public GameObject mainCamera;
    //public GameObject[] panels;

	// Network variables
	private GameObject networkManager;
	private NetworkManager networkManagerScript;
	private HostData[] hostList = null;
    private bool isDone = false;

    public GameObject CameraVR;
    public GameObject CameraStatic;
    public GameObject Racer;

    public Text m_Msg;
    
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

        // If the application detect an Oculus => Try to create an host
        if(UnityEngine.VR.VRDevice.isPresent)
        {
            CameraStatic.SetActive(false);
            OnCreateRoom();
        }
        else // Try to join a room
        {
            CameraVR.SetActive(false);
            Racer.SetActive(false);
            OnJoinRoom();
        }

	}


    /*******************************************************
	 * DISPLAY
	 ******************************************************/
    public void DisplayMessage(string msg)
    {
        m_Msg.text = msg;
    }

    /*******************************************************
    * EVENTS
    ******************************************************/
    public void OnCreateRoom()
    {
        DisplayMessage("Intialisation du serveur");
        networkManagerScript.StartServer();
    }

    public void OnJoinRoom()
    {
        DisplayMessage("Recherche du serveur");
        currentMenu = MenuState.RoomList;
        RequestRoomList();
    }

    /*
    public void OnBack()
    {
        currentMenu = MenuState.TwoPlayers;
        DisplayCurrentMenu();
        networkManagerScript.CloseServer();
    }*/

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
            }
        }

    }

    public void OnJoinServer()
    {
        //Debug.Log("SERVER : " + i--);
        //Debug.Log("SERVER : " + i--);
        DisplayMessage("Connexion au serveur " + hostList[0].gameName);
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
            OnJoinServer();
        }
	}

	public void setCurrentStateWait(){
        DisplayMessage("Serveur en attente d'un client");
        currentMenu = MenuState.WaitingRoom;
    }
	public void setCurrentStateNetwork(){
		currentMenu = MenuState.TwoPlayers;
       // DisplayCurrentMenu();
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
