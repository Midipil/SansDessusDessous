using UnityEngine;
using System.Collections;

/**
 * NetworkControl handles the players actions on the two Dragons movements on the Network
 * ----------------------------------------------
 * Need the Component>Miscellaneous>NetworkView to be add to the Dragon object,
 * which enable us to send data packages over the network to synchronise the player
 * Parameters : 
 * - "State synchronized" set to "reliable delta compressed" 
 * 	 synchronized data will be sent automatically, but only if its value changed
 * - "Observed" contains the component that will be synchronized.
 * 	 The Object Transform is automatically added to this field
 * 	 In order to write our own synchronization method, we drag the 
 * 	 component of the player script into this field
 * -----------------------------------------------
 * The Dragon object need to be add to the bierachy to make it a prefab
 * that can be instantiate on the network
 * -----------------------------------------------
 * Run a server and client on the same computer :
 * Edit -> Project Settings -> Player -> “Run in Background” 
 *(Else you won’t be able to connect to the server, unless you focus on the game instance running the server after client has been connected)
 */
public class NetworkEnemyController : MonoBehaviour {

    // Network variables
    private GameObject networkManager;
    private NetworkManager networkManagerScript;

    public GameObject myCamera;

    // Interpolation values
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;

    private Quaternion syncStartRotation = Quaternion.identity;
    private Quaternion syncEndRotation = Quaternion.identity;

    private NetworkView m_networkView;

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }

    void Start()
    {

        m_networkView = this.GetComponent<NetworkView>();

        // If the EnnemyInstance isn't the player's one, disable its camera
        if (!m_networkView.isMine)
        {
            myCamera.gameObject.SetActive(false);
        }

        // Find the NetworkManager object
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        networkManagerScript = networkManager.GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update () {
        // The input functions are only called if the ClientInstance is the player's one
        if (m_networkView.isMine)
        {
            InputMovement();
        }
        // When it's not, we need to use the interpolation between the synchronized values
        // to update the ClientInstance mouvement according to the movements of the Character of the player's opponent
        else
        {
            SyncedMovement();
        }		
	}

    /*Handle Client player's input*/
    void InputMovement()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Enemy_Vertical"), Input.GetAxis("Enemy_Horizontal"), 0));
    }

    /* Update the rotation and velocity of the ClientInstance according to the movements of the Character of the player's opponent*/
    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;

        transform.localRotation = Quaternion.Slerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
    }

    /* This function is automatically called every time it sends or receives datas.
(To use for data that constantly changed) */
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Quaternion syncRotation = Quaternion.identity;

        // The player is writing to the stream (= he moves its own Character...)
        if (stream.isWriting)
        {
            syncRotation = transform.localRotation;
            stream.Serialize(ref syncRotation);
        }
        // The ClientInstance of the player's opponent need to be moved
        else
        { 
            stream.Serialize(ref syncRotation);

            // Interpolation : smoothing the transition from the old to the new data values
            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            // Prediction : the rotation is "updated" before the new data is received        
            syncStartRotation = transform.localRotation;
            syncEndRotation = syncRotation;
        }
    }
}
