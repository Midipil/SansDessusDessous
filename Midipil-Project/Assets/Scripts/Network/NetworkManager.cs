using UnityEngine;
using System.Collections;

/**
 * NetworkManager handles hosting a server or connecting to an existing host
 */
public class NetworkManager : MonoBehaviour
{
	// Main menu variables
	private NetworkConnexionProcess mainMenuScript;
	private bool hasMessageToMenu = false;
	private string messageToMenu;

	// Network variables
	private const string typeName = "LaCouveuseVR";
	private const string gameName = "RunnerVSGuardian";

    private bool isRefreshingHostListNeeded = false;
    private HostData[] hostList = null;
	

	/*******************************************************
	 * Initialisation functions
	 ******************************************************/
	public void Intialize(NetworkConnexionProcess l_mainMenuScript)
    {
		// Retrieve the MainMenu gameObject
		mainMenuScript = l_mainMenuScript;

		// The game was played and the server or the client has quitted
		if (hasMessageToMenu) {
			mainMenuScript.setCurrentStateHome();
			mainMenuScript.setMessage(messageToMenu);
			hasMessageToMenu = false;
		}
	}

	/*******************************************************
	 * Server actions : Start or Close a server
	 ******************************************************/
    public void StartServer()
    {
		// Initialize the server on the network : 
		// InitializeServer(MaxPlayerAmount, PortNumber, Use NAT punchthrough if no public IP present)
		NetworkConnectionError error  = Network.InitializeServer(2, 25000, !Network.HavePublicAddress());
		if (error != NetworkConnectionError.NoError)
            mainMenuScript.setMessage("The Runner has already been chosen.\n Try to play as the Guardian to join the game !");
            //Debug.Log (error);
            //mainMenuScript.setMessage("A server has already been started by another player.\n Try to play as the Guardian to join the game !");
            // Register the host to the Master : ServerRegisterHost(UniqueGameName, RoomName)
            MasterServer.RegisterHost(typeName, gameName);
    }
	
	public void CloseServer(){
		Network.Disconnect();
		MasterServer.UnregisterHost();
        MasterServer.ClearHostList();
    }

	public void CloseServerInGame(){
		// Kick the player off the server before closing it (see OnPlayerDisconnected() function)
		if (Network.connections.Length > 0) {
			Debug.Log("Disconnecting: "+
			          Network.connections[0].ipAddress+":"+Network.connections[0].port);
			Network.CloseConnection(Network.connections[0], true);
		}
    }


	/*******************************************************
	 * Server and Client actions : quit the game and go back to the menu
	 ******************************************************/
	public void QuitGame(){
		// Properly close or quit the server
		if (Network.isServer){
			messageToMenu = "The server has been successfully closed";
			hasMessageToMenu = true;
			CloseServerInGame();
		}
		else{
			messageToMenu = "You have been successfully disconnected from the server";
			hasMessageToMenu = true;
			QuitServer();
		}
        
	}

	/*******************************************************
	 * Client actions : Join or Quit a server
	 ******************************************************/
	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	public void QuitServer()
	{
		Network.Disconnect();
	}


	/*******************************************************
	 * Update the host list
	 ******************************************************/
    void Update()
    {
      
        if (MasterServer.PollHostList().Length != 0)
        {
            hostList = MasterServer.PollHostList();
            int i = 0;
            Debug.Log("Print List");
            while (i < hostList.Length)
            {
                Debug.Log("Game name: " + hostList[i].gameName);
                i++;
            }

            if (hostList.Length > 0)
            {
                // The MainMenu list of hosts need to be refreshed as well
                mainMenuScript.setHostList(hostList);
            }
            isRefreshingHostListNeeded = false;
            MasterServer.ClearHostList();
        }

        if (isRefreshingHostListNeeded)
        {
            //isRefreshingHostList = true;
            // RequestHostList() empties the stored HostData array until the OnMasterServerMessage callback triggers with a HostListReceived event.
            MasterServer.RequestHostList(typeName);
        }

            /*
            // If the list of host has been refreshed 
            if (isRefreshingHostList)
            {
                isRefreshingHostList = false;
                // If the list of host isn't empty, the host list is refresh
                if (MasterServer.PollHostList().Length > 0)
                {
                    hostList = MasterServer.PollHostList();
                    Debug.Log("RefreshServerList:" + hostList.Length);
                    foreach (HostData data in hostList)
                    {
                        Debug.Log("HostData:" + data.ToString());
                    }
                }
                else
                {
                    hostList = null;
                    Debug.Log("LIST VIDE :(");
                }

                // The MainMenu list of hosts need to be refreshed as well
                mainMenuScript.setHostList(hostList);
            }
            */
    }

	// Send a request to the master server to get the list of host contening all the data to join a server
    public void RefreshHostList()
    {
        //if (!isRefreshingHostList)
        //{
            isRefreshingHostListNeeded = true;
            //isRefreshingHostList = true;
            // RequestHostList() empties the stored HostData array until the OnMasterServerMessage callback triggers with a HostListReceived event.
            MasterServer.RequestHostList(typeName);
        //}
    }


	/***************************************************************************
	 * Messages sent on the server or the client when a specific event occures
	 **************************************************************************/	
	// Actions called on the server whenever it has been succesfully initialized
	void OnServerInitialized()
	{
        // Display the waiting room for the server-player
        mainMenuScript.setCurrentStateWait();
	}

    void OnRegistrationSucceeded()
    {
        Debug.Log("SERVER REGISTERED");
    }

    //RequestHostList() empties the stored HostData array until the OnMasterServerMessage callback triggers with a HostListReceived event.s
    /*void OnHostListReceived()
    {

    }*/

    // Actions called on the client when a connection attempt fails for some reason
    void OnFailedToConnect(){
	}

	// Actions called on the client when it has successfully joined a server
    void OnConnectedToServer()
    {
		// Launch the game
		mainMenuScript.PlayAsClient();
    }
	// Actions called on the server whenever a new player has successfully connected
	void OnPlayerConnected()
	{
		// Launch the game
		mainMenuScript.PlayAsServer();
	}

	// Actions called on the server whenever a player is disconnected from the server
	void OnPlayerDisconnected(){
        Debug.Log("DISCONNECTED");
		if(!hasMessageToMenu){
			messageToMenu = "The Guardian has quit the game"; // The client has quit
			hasMessageToMenu = true;
		}
		CloseServer ();
		// Don't destroy the game object on which the script is attached
		DontDestroyOnLoad (gameObject);
		//Load the menu
		Application.LoadLevel ("ConnexionFull");
	}

	// Actions called on client during disconnection from server, but also on the server when the connection has disconnected
	void OnDisconnectedFromServer(){
		if(Network.isClient){
			if(!hasMessageToMenu){
			   messageToMenu = "The server has been closed.\nThe Runner has quit the game.";
			   hasMessageToMenu = true;
			}
			// Don't destroy the game object on which the script is attached
			DontDestroyOnLoad (gameObject);
			// Load the menu
			Application.LoadLevel ("ConnexionFull");
		}
	}

}
