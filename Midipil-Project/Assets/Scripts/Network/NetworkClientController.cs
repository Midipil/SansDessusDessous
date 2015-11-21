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
public class NetworkClientController : MonoBehaviour {

	// Network variables
	private GameObject networkManager;
	private NetworkManager networkManagerScript;
	
	public GameObject myCamera;

    private float speed = 10f;

	// Interpolation values
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;

	private float forceMult = 0f;
	private float syncForceMult = 0f;

	private Vector3 newRot = Vector3.zero;
	private Vector3 syncNewRot = Vector3.zero;

	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;

    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private NetworkView m_networkView;

	void Awake()
	{
		lastSynchronizationTime = Time.time;
	}

	void Start(){

        m_networkView = this.GetComponent<NetworkView>();

        // If the ClientInstance isn't the player's one, disable its camera
        if (!m_networkView.isMine) {
            myCamera.gameObject.SetActive(false);
		}
        /*
        // If the ClientInstance is the player's one, get the leapController
        if (networkView.isMine){
			m_leapController = new Controller();
		}*/

		// Find the NetworkManager object
		networkManager = GameObject.FindGameObjectWithTag ("NetworkManager");
		networkManagerScript = networkManager.GetComponent<NetworkManager>();
	}


	void Update()
	{
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

    //===================================================
    // HANDLE CHANGE COLOR EXAMPLE
    /*
    private void InputColorChange()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    [RPC]
    void ChangeColorTo(Vector3 color)
    {
        this.GetComponent<Renderer>().color = new Color(color.x, color.y, color.z, 1f);

        if (networkView.isMine)
            networkView.RPC("ChangeColorTo", RPCMode.OthersBuffered, color); // OBSCOLETE  :/
    }


    //===================================================
    // HANDLE COLLSIION WITH RIGIDBODY EXAMPLE
    /*
    // On ClientInstance collision : Send the new position, rotation and velocity compute by the collision engine
	void OnCollisionEnter(Collision collision)
    {
        if (networkView.isMine)
        {
            networkView.RPC("updatePosition", RPCMode.OthersBuffered, transform.localPosition, transform.localRotation, transform.rigidbody.velocity);
        }
    }

    // Update the position, rotation and velocity of the other user's Character after a collision
    [RPC] void updatePosition(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
        transform.rigidbody.velocity = velocity;
    }
    */

    //===================================================

    /* Update the rotation and velocity of the ClientInstance according to the movements of the Character of the player's opponent*/
    private void SyncedMovement()
	{
		syncTime += Time.deltaTime;


        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);

        transform.localRotation = Quaternion.Slerp(syncStartRotation, syncEndRotation, syncTime /syncDelay);
        //this.GetComponent<Rigidbody>().velocity = transform.forward * syncForceMult;

    }

	/*Handle Client player's input*/
	void InputMovement() {

        // Move Up and down
        if (Input.GetKey(KeyCode.UpArrow))
        {
           this.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(-Vector3.up * speed * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(newRot), 0.1f);
        //this.GetComponent<Rigidbody>().velocity = transform.forward * forceMult;
    }


	/* This function is automatically called every time it sends or receives datas.
	(To use for data that constantly changed) */
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Quaternion syncRotation = Quaternion.identity;
		//Vector3 syncVelocity = Vector3.zero;
        Vector3 syncPosition = Vector3.zero;

        // The player is writing to the stream (= he moves its own Character...)
        if (stream.isWriting)
		{
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncRotation = transform.localRotation;
			stream.Serialize(ref syncRotation);


            /*
            syncVelocity = this.GetComponent<Rigidbody>()..velocity;
			stream.Serialize(ref syncVelocity);

			syncForceMult = forceMult;
			stream.Serialize(ref syncForceMult);
            */

			syncNewRot = newRot;
			stream.Serialize(ref syncNewRot);

		}
		// The ClientInstance of the player's opponent need to be moved
		else
		{
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncRotation);
            //stream.Serialize(ref syncVelocity);
            //stream.Serialize(ref syncForceMult);
            stream.Serialize(ref syncNewRot);


            // Interpolation : smoothing the transition from the old to the new data values
            syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

            syncStartPosition = transform.position;
            syncEndPosition = syncPosition;

            // Prediction : the rotation is "updated" before the new data is received
            syncEndRotation = Quaternion.Slerp(syncRotation, Quaternion.Euler(syncNewRot), syncDelay);
            syncStartRotation = transform.localRotation;
            // Using rigidbody instead transform for the position
            /*
            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = rigidbody.position;
            */
        }
	}
}
