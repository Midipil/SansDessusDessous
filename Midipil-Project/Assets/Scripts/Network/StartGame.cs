using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
    public GameObject serverView;
    public GameObject clientView;

    // Use this for initialization
    void Start()
    {
        serverView.SetActive(false);
        clientView.SetActive(false);

        if (Network.isServer)
        {
            Debug.LogError("LOAD SERVER SCENE !");
            serverView.SetActive(true);
        }
        else
        {
            Debug.LogError("LOAD CLIENT SCENE !");
            clientView.SetActive(true);
        }
    }
}
