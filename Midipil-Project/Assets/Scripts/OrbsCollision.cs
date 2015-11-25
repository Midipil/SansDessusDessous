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
			Destroy(other.gameObject);
			orbsDestroyed++;
			//Debug.Log("Orb destroyed ("+orbsDestroyed+"/"+orbsNum+" - "+orbsToDestroy+" mini to win");
			// Play sound
			this.transform.Find("Bearbot-vaisseau").GetComponent<AudioSource>().Play();
			// Give boost
			player.GetComponent<PlayerController>().Boost(); 

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
