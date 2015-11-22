using UnityEngine;
using System.Collections;

public class NetworkStarter : MonoBehaviour {

    public GameObject playerSolo;
    public GameObject ennemySolo;

    public GameObject playerViewPrefab;
    public GameObject ennemyViewPrefab;

    // Use this for initialization
    void Start() {
        if (GameObject.FindGameObjectWithTag("NetworkManager") == null)
        {
            // SOLO
        }
        else
        {
            // MULTI 
            playerSolo.SetActive(false);
            ennemySolo.SetActive(false);

            if (Network.isServer)
            {
                Debug.LogError("LOAD PLAYER SCENE !");
                Network.Instantiate(playerViewPrefab, playerViewPrefab.transform.position, playerViewPrefab.transform.rotation, 0);
            }
            else
            {
                Debug.LogError("LOAD ENNEMY SCENE !");
                Network.Instantiate(ennemyViewPrefab, ennemyViewPrefab.transform.position, ennemyViewPrefab.transform.rotation, 0);
            }
        }
    }
}

