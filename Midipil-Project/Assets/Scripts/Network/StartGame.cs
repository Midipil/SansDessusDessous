using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
    public GameObject serverViewPrefab;
    public GameObject clientViewPrefab;

    // Use this for initialization
    void Start()
    {

        if (Network.isServer)
        {
            Debug.LogError("LOAD SERVER SCENE !");
            Network.Instantiate(serverViewPrefab, serverViewPrefab.transform.position, serverViewPrefab.transform.rotation, 0);
        }
        else
        {
            Debug.LogError("LOAD CLIENT SCENE !");
            Network.Instantiate(clientViewPrefab, clientViewPrefab.transform.position, clientViewPrefab.transform.rotation, 0);
        }
    }
}
