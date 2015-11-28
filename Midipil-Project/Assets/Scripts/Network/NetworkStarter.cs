using UnityEngine;
using System.Collections;

public class NetworkStarter : MonoBehaviour {

    public GameObject playerSolo;
    public GameObject ennemySolo;

    public GameObject playerViewPrefab;
    public GameObject ennemyViewPrefab;

    private NetworkManager networkManagerScript;

    // Use this for initialization
    void Start() {
        GameObject networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        if (networkManager == null)
        {
            // SOLO
        }
        else
        {
            networkManagerScript = networkManager.GetComponent<NetworkManager>();

            // MULTI 
            playerSolo.SetActive(false);
            ennemySolo.SetActive(false);

            if (Network.isServer)
            {
                Debug.Log("LOAD PLAYER SCENE !");
                Network.Instantiate(playerViewPrefab, playerViewPrefab.transform.position, playerViewPrefab.transform.rotation, 0);
            }
            else
            {
                Debug.Log("LOAD ENNEMY SCENE !");
                Network.Instantiate(ennemyViewPrefab, ennemyViewPrefab.transform.position, ennemyViewPrefab.transform.rotation, 0);
            }
        }
    }

    void Update()
    {
        // QUIT
        if (networkManagerScript && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Quit")))
        {
            networkManagerScript.QuitGame();
        }
    }
}

