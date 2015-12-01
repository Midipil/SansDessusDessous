using UnityEngine;
using System.Collections;

public class OrbsCollision : MonoBehaviour {

	private int orbsNum; // Set in orbsmanager
	public int orbsToDestroy = 20;
	private int orbsDestroyed = 0;

	private GameObject player;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
		//orbsNum = GameObject.Find("Orbs").GetComponent<OrbsManager>().getOrbsNum();
        orbsNum = GameObject.Find("OrbsInstanciated3").transform.childCount;
        Debug.Log(orbsNum);
    }
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider other) {
		// Get orb component
		GameObject go = other.transform.gameObject;
		Orb o = go.GetComponent<Orb>();
		
		if (!o.isTriggered()){
			o.setTriggered();
			Destroy(other.transform.parent.gameObject);
			orbsDestroyed++;
			//Debug.Log("Orb destroyed ("+orbsDestroyed+"/"+orbsNum+" - "+orbsToDestroy+" mini to win");
			// Play sound
			this.transform.Find("Bearbot-vaisseau").GetComponent<AudioSource>().Play();
			// Give boost
			if(player.GetComponent<NetworkPlayerController>() != null){
				player.GetComponent<NetworkPlayerController>().Boost();
			} else if(player.GetComponent<PlayerController>() != null){
				player.GetComponent<PlayerController>().Boost();
			} else {
				Debug.LogError("Can't find player controller component to apply boost");
			}

			if (orbsDestroyed >= orbsToDestroy){
				// End of game
				win();
			}
		}
	}
	
	void win(){
		Debug.Log("Player win");
		GameObject.FindWithTag("GameManager").GetComponent<GameManager>().playerWin = true;
	}
}
